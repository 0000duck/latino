using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Latino.TextMining
{
    public class MultiLangBowSpace : BowSpace
    {
        private static Logger mLogger = Logger.GetLogger(typeof(MultiLangBowSpace));

        Dictionary<string, IStemmer> stemmers = new Dictionary<string, IStemmer>();
        Dictionary<string, Set<string>.ReadOnly> stopWordDict = new Dictionary<string, Set<string>.ReadOnly>();

        public void Initialize(IEnumerable<KeyDat<string, string>> documents)
        {
            Initialize(documents, /*keepBowVectors=*/true);
        }

        public ArrayList<SparseVector<double>> Initialize(IEnumerable<KeyDat<string, string>> documents, bool keepBowVectors)
        {
            Utils.ThrowException(documents == null ? new ArgumentNullException("documents") : null);
            Debug.Assert(documents != null, "Documents are always passed");
            mWordInfo.Clear();
            mIdxInfo.Clear();
            mBowVectors.Clear();
            ArrayList<SparseVector<double>> bows = keepBowVectors ? null : new ArrayList<SparseVector<double>>();
            // build vocabulary
            mLogger.Info("Initialize", "Building vocabulary ...");
            int docCount = 0;
            foreach (var document in documents)
            {
                docCount++;
                mLogger.ProgressFast(this, "Initialize", "Document {0} ...", docCount, /*numSteps=n*/-1);
                Set<string> docWords = new Set<string>();
                ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
                mTokenizer.Text = document.First;

                IStemmer stemmer;
                Set<string>.ReadOnly stopWords;
                // Setup stopwords and stemmer
                if (stemmers.ContainsKey(document.Second))
                {
                    stemmer = stemmers[document.Second];
                    stopWords = stopWordDict[document.Second];
                }
                else
                {
                    Language lang = TextMiningUtils.GetLanguage(document.Second);

                    try
                    {
                        TextMiningUtils.GetLanguageTools(lang, out stopWords, out stemmer);
                    }
                    catch (ArgumentNotSupportedException)   // Language tools to not exist, so fallback to english
                    {
                        TextMiningUtils.GetLanguageTools(Language.English, out stopWords, out stemmer);
                        mLogger.Error("Initialize", "Missing language tools for language code {0}.", document.Second);
                    }

                    stemmers[document.Second] = stemmer;
                    stopWordDict[document.Second] = stopWords;
                }

                foreach (string token in mTokenizer)
                {
                    string word = token.Trim().ToLower();
                    if (stopWords == null || !stopWords.Contains(word))
                    {
                        string stem = stemmer == null ? word : stemmer.GetStem(word).Trim().ToLower();
                        if (nGrams.Count < mMaxNGramLen)
                        {
                            WordStem wordStem = new WordStem();
                            wordStem.Word = word;
                            wordStem.Stem = stem;
                            nGrams.Add(wordStem);
                            if (nGrams.Count < mMaxNGramLen)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            WordStem wordStem = nGrams[0];
                            wordStem.Word = word;
                            wordStem.Stem = stem;
                            for (int i = 0; i < mMaxNGramLen - 1; i++)
                            {
                                nGrams[i] = nGrams[i + 1];
                            }
                            nGrams[mMaxNGramLen - 1] = wordStem;
                        }
                        ProcessNGramsPass1(nGrams, 0, docWords);
                    }
                }
                int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
                for (int i = startIdx; i < nGrams.Count; i++)
                {
                    ProcessNGramsPass1(nGrams, i, docWords);
                }
            }
            mLogger.ProgressFast(this, "Initialize", "Document {0} ...", docCount, docCount);
            // remove unfrequent words and n-grams, precompute IDF      
            ArrayList<string> removeList = new ArrayList<string>();
            foreach (KeyValuePair<string, Word> wordInfo in mWordInfo)
            {
                if (wordInfo.Value.mFreq < mMinWordFreq)
                {
                    removeList.Add(wordInfo.Key);
                }
                else
                {
                    wordInfo.Value.mIdf = Math.Log((double)docCount / (double)wordInfo.Value.mDocFreq);
                }
            }
            foreach (string key in removeList) { mWordInfo.Remove(key); }
            // determine most frequent word and n-gram forms
            foreach (Word wordInfo in mWordInfo.Values)
            {
                int max = 0;
                foreach (KeyValuePair<string, int> wordForm in wordInfo.mForms)
                {
                    if (wordForm.Value > max)
                    {
                        max = wordForm.Value;
                        wordInfo.mMostFrequentForm = wordForm.Key;
                    }
                }
                if (!mKeepWordForms) { wordInfo.mForms.Clear(); }
            }
            // compute bag-of-words vectors
            mLogger.Info("Initialize", "Computing bag-of-words vectors ...");
            int docNum = 1;
            foreach (var document in documents)
            {
                Set<string>.ReadOnly stopWords = stopWordDict[document.Second];
                IStemmer stemmer = stemmers[document.Second];

                mLogger.ProgressFast(this, "Initialize", "Document {0} / {1} ...", docNum++, docCount);
                Dictionary<int, int> tfVec = new Dictionary<int, int>();
                ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
                mTokenizer.Text = document.First;
                foreach (string token in mTokenizer)
                {
                    string word = token.Trim().ToLower();
                    if (stopWords == null || !stopWords.Contains(word))
                    {
                        string stem = stemmer == null ? word : stemmer.GetStem(word).Trim().ToLower();
                        if (nGrams.Count < mMaxNGramLen)
                        {
                            WordStem wordStem = new WordStem();
                            wordStem.Word = word;
                            wordStem.Stem = stem;
                            nGrams.Add(wordStem);
                            if (nGrams.Count < mMaxNGramLen) { continue; }
                        }
                        else
                        {
                            WordStem wordStem = nGrams[0];
                            wordStem.Word = word;
                            wordStem.Stem = stem;
                            for (int i = 0; i < mMaxNGramLen - 1; i++) { nGrams[i] = nGrams[i + 1]; }
                            nGrams[mMaxNGramLen - 1] = wordStem;
                        }
                        ProcessNGramsPass2(nGrams, 0, tfVec);
                    }
                }
                int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
                for (int i = startIdx; i < nGrams.Count; i++)
                {
                    ProcessNGramsPass2(nGrams, i, tfVec);
                }
                SparseVector<double> docVec = new SparseVector<double>();
                if (mWordWeightType == WordWeightType.TermFreq)
                {
                    foreach (KeyValuePair<int, int> tfItem in tfVec)
                    {
                        docVec.InnerIdx.Add(tfItem.Key);
                        docVec.InnerDat.Add(tfItem.Value);
                    }
                }
                else if (mWordWeightType == WordWeightType.TfIdf)
                {
                    foreach (KeyValuePair<int, int> tfItem in tfVec)
                    {
                        double tfIdf = (double)tfItem.Value * mIdxInfo[tfItem.Key].mIdf;
                        if (tfIdf > 0)
                        {
                            docVec.InnerIdx.Add(tfItem.Key);
                            docVec.InnerDat.Add(tfIdf);
                        }
                    }
                }
                else if (mWordWeightType == WordWeightType.LogDfTfIdf)
                {
                    foreach (KeyValuePair<int, int> tfItem in tfVec)
                    {
                        double tfIdf = (double)tfItem.Value * mIdxInfo[tfItem.Key].mIdf;
                        if (tfIdf > 0)
                        {
                            docVec.InnerIdx.Add(tfItem.Key);
                            docVec.InnerDat.Add(Math.Log(1 + mIdxInfo[tfItem.Key].mDocFreq) * tfIdf);
                        }
                    }
                }
                docVec.Sort();
                CutLowWeights(ref docVec);
                if (mNormalizeVectors) { Utils.TryNrmVecL2(docVec); }
                if (keepBowVectors) { mBowVectors.Add(docVec); }
                else { bows.Add(docVec); }
            }
            return bows; 

        }

        public SparseVector<double> ProcessDocument(string document, string language)
        {
            Dictionary<int, int> tfVec = new Dictionary<int, int>();
            ArrayList<WordStem> nGrams = new ArrayList<WordStem>(mMaxNGramLen);
            mTokenizer.Text = document;

            IStemmer stemmer;
            Set<string>.ReadOnly stopWords;

            if (stemmers.ContainsKey(language))
            {
                stemmer = stemmers[language];
                stopWords = stopWordDict[language];
            }
            else
            {
                Language lang = TextMiningUtils.GetLanguage(language);

                try
                {
                    TextMiningUtils.GetLanguageTools(lang, out stopWords, out stemmer);
                }
                catch (ArgumentNotSupportedException)   // Language tools to not exist, so fallback to english
                {
                    TextMiningUtils.GetLanguageTools(Language.English, out stopWords, out stemmer);
                    mLogger.Error("Initialize", "Missing language tools for language code {0}.", language);
                }

                stemmers[language] = stemmer;
                stopWordDict[language] = stopWords;
            }

            foreach (string token in mTokenizer)
            {
                string word = token.Trim().ToLower();
                if (stopWords == null || !stopWords.Contains(word))
                {
                    string stem = stemmer == null ? word : stemmer.GetStem(word).Trim().ToLower();
                    if (nGrams.Count < mMaxNGramLen)
                    {
                        WordStem wordStem = new WordStem();
                        wordStem.Word = word;
                        wordStem.Stem = stem;
                        nGrams.Add(wordStem);
                        if (nGrams.Count < mMaxNGramLen) { continue; }
                    }
                    else
                    {
                        WordStem wordStem = nGrams[0];
                        wordStem.Word = word;
                        wordStem.Stem = stem;
                        for (int i = 0; i < mMaxNGramLen - 1; i++) { nGrams[i] = nGrams[i + 1]; }
                        nGrams[mMaxNGramLen - 1] = wordStem;
                    }
                    ProcessDocumentNGrams(nGrams, 0, tfVec);
                }
            }
            int startIdx = nGrams.Count == mMaxNGramLen ? 1 : 0;
            for (int i = startIdx; i < nGrams.Count; i++)
            {
                ProcessDocumentNGrams(nGrams, i, tfVec);
            }
            SparseVector<double> docVec = new SparseVector<double>();
            if (mWordWeightType == WordWeightType.TermFreq)
            {
                foreach (KeyValuePair<int, int> tfItem in tfVec)
                {
                    docVec.InnerIdx.Add(tfItem.Key);
                    docVec.InnerDat.Add(tfItem.Value);
                }
            }
            else if (mWordWeightType == WordWeightType.TfIdf)
            {
                foreach (KeyValuePair<int, int> tfItem in tfVec)
                {
                    double tfIdf = (double)tfItem.Value * mIdxInfo[tfItem.Key].mIdf;
                    if (tfIdf > 0)
                    {
                        docVec.InnerIdx.Add(tfItem.Key);
                        docVec.InnerDat.Add(tfIdf);
                    }
                }
            }
            else if (mWordWeightType == WordWeightType.LogDfTfIdf)
            {
                foreach (KeyValuePair<int, int> tfItem in tfVec)
                {
                    double tfIdf = (double)tfItem.Value * mIdxInfo[tfItem.Key].mIdf;
                    if (tfIdf > 0)
                    {
                        docVec.InnerIdx.Add(tfItem.Key);
                        docVec.InnerDat.Add(Math.Log(1 + mIdxInfo[tfItem.Key].mDocFreq) * tfIdf);
                    }
                }
            }
            docVec.Sort();
            CutLowWeights(ref docVec);
            if (mNormalizeVectors) { Utils.TryNrmVecL2(docVec); }
            return docVec;
        }
    }
}
