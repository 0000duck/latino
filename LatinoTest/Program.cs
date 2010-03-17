using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Latino;
using Latino.TextMining;
using Latino.Model;
using Latino.Visualization;
using System.Windows.Forms;
using System.Drawing;
using System.Web;
using System.Collections;

namespace BDocVisualizer
{
    public class Document : Ellipse
    {
        public int Idx
            = -1;
        public string Text
            = "";
        public string Label
            = "";
        public string OppositeKeywords
            = "";

        public Document(float x, float y, float rX, float rY) : base(x, y, rX, rY)
        {
        }
    }

    public static class Program
    {
        private static SparseVector<double> GetProdVec(SparseVector<double>.ReadOnly a, SparseVector<double>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            SparseVector<double> prodVec = new SparseVector<double>();
            int i = 0, j = 0;
            int aCount = a.Count;
            int bCount = b.Count;
            if (aCount == 0 || bCount == 0) { return prodVec; }
            ArrayList<int> aIdx = a.Inner.InnerIdx;
            ArrayList<double> aDat = a.Inner.InnerDat;
            ArrayList<int> bIdx = b.Inner.InnerIdx;
            ArrayList<double> bDat = b.Inner.InnerDat;
            int aIdxI = aIdx[0];
            int bIdxJ = bIdx[0];
            while (true)
            {
                if (aIdxI < bIdxJ)
                {
                    if (++i == aCount) { break; }
                    aIdxI = aIdx[i];
                }
                else if (aIdxI > bIdxJ)
                {
                    if (++j == bCount) { break; }
                    bIdxJ = bIdx[j];
                }
                else
                {
                    prodVec.InnerIdx.Add(aIdxI);
                    prodVec.InnerDat.Add(aDat[i] * bDat[j]);
                    if (++i == aCount || ++j == bCount) { break; }
                    aIdxI = aIdx[i];
                    bIdxJ = bIdx[j];
                }
            }
            return prodVec;
        }

        private static bool IsAllStopWords(string term, Set<string>.ReadOnly stopWords)
        {
            string[] words = term.Split(' ');
            foreach (string word in words)
            {
                if (!stopWords.Contains(word)) { return false; }
            }
            return true;
        }

        private static bool ContainsGoldenStandardWord(string term, Set<string>.ReadOnly goldenStandard, IStemmer stemmer)
        {
            string[] words = term.Split(' ');
            foreach (string word in words)
            {
                if (goldenStandard.Contains(stemmer.GetStem(word))) { return true; }
            }
            return false;
        }

        private static bool ContainsGoldenStandardWord2(string term, Set<string> goldenStandard, IStemmer stemmer)
        {
            string[] words = term.Split(' ');
            bool retVal = false;
            foreach (string word in words)
            {
                string stem = stemmer.GetStem(word);
                if (goldenStandard.Contains(stem))
                {
                    retVal = true;
                    goldenStandard.Remove(stem);
                }
            }
            return retVal;
        }

        private static string TermsToString(IEnumerable<string> trems, Set<string>.ReadOnly stopWords, Set<string>.ReadOnly goldenStandard, IStemmer stemmer)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (string term in trems)
            {
                if (IsAllStopWords(term, stopWords)) { strBuilder.Append("(" + term + "), "); }
                else if (ContainsGoldenStandardWord(term, goldenStandard, stemmer)) { strBuilder.Append(term + "*, "); }
                else { strBuilder.Append(term + ", "); }
            }
            return strBuilder.ToString().TrimEnd(' ', ',');
        }

        private static void TermsToStringCsv(IEnumerable<KeyDat<double, string>> trems, Set<string>.ReadOnly stopWords, Set<string>.ReadOnly goldenStandard, IStemmer stemmer, string fileName)
        {
            StreamWriter writer = new StreamWriter(fileName);
            StringBuilder strBuilder = new StringBuilder();
            foreach (KeyDat<double, string> term in trems)
            {   
                if (true/*!IsAllStopWords(term.Dat, stopWords)*/) { strBuilder.Append(string.Format("{0},", term.Key)); }
                if (IsAllStopWords(term.Dat, stopWords)) { strBuilder.AppendLine(term.Dat + ",,STOP"); }
                else if (ContainsGoldenStandardWord(term.Dat, goldenStandard, stemmer)) { strBuilder.AppendLine(term.Dat + ",GOLD,"); }
                else { strBuilder.AppendLine(term.Dat); }
            }
            writer.WriteLine(strBuilder.ToString());
            writer.Close();
        }

        private static string TermsToString2(IEnumerable<string> trems, Set<string>.ReadOnly stopWords, Set<string> goldenStandard, IStemmer stemmer)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (string term in trems)
            {
                if (IsAllStopWords(term, stopWords)) { /*strBuilder.Append("(" + term + "), ");*/ }
                else if (ContainsGoldenStandardWord2(term, goldenStandard, stemmer)) { strBuilder.Append(term + "*, "); }
                else { strBuilder.Append(term + ", "); }
            }
            return strBuilder.ToString().TrimEnd(' ', ',');
        }

        private static int GetWordIdx(string wordStem, BowSpace bowSpace)
        {
            int i = 0;
            foreach (Word word in bowSpace.Words)
            {
                if (bowSpace.Stemmer.GetStem(word.MostFrequentForm) == bowSpace.Stemmer.GetStem(wordStem)) { return i; }
                i++;
            }
            return -1;
        }

        private static void ComputeBisociativity(IEnumerable<string> docs, ITokenizer tokenizer, IStemmer stemmer, Set<string>.ReadOnly stopWords, Set<string>.ReadOnly goldenStandard)
        {
            Console.WriteLine("-> Computing bisociativity ...");
            // compute pure TF vectors
            BowSpace bowSpace = new BowSpace();
            bowSpace.StopWords = StopWords.EnglishStopWords;
            bowSpace.Stemmer = stemmer;
            bowSpace.WordWeightType = WordWeightType.TermFreq;
            bowSpace.NormalizeVectors = false;
            bowSpace.MinWordFreq = 1;
            bowSpace.MaxNGramLen = 1;// 3;
            bowSpace.CutLowWeightsPerc = 0;
            bowSpace.Tokenizer = tokenizer;
            bowSpace.Initialize(docs);
            // normalize TF vectors         
            foreach (SparseVector<double>.ReadOnly vec in bowSpace.BowVectors)
            {
                double sum = 0;
                foreach (IdxDat<double> item in vec) 
                { 
                    sum += item.Dat; 
                }
                if (sum != 0)
                {
                    int i = 0;
                    foreach (IdxDat<double> item in vec)
                    {
                        vec.Inner.SetDirect(i++, item.Dat / sum);
                    }
                }
            }
            // transpose matrix
            SparseMatrix<double> mat = ModelUtils.GetTransposedMatrix(bowSpace);
            // compute bisociativity values
            StreamWriter writer = new StreamWriter("..\\..\\data\\swansonBisoc.csv");
            Bisociativity bisoc = new Bisociativity(/*k=*/1);
            int migraineIdx = GetWordIdx("migraine", bowSpace);
            int magnesiumIdx = GetWordIdx("magnesium", bowSpace);
            int j = 0;
            writer.WriteLine("Word,IsStopWord,IsGoldenStandardWord,Migraine,Magnesium,Min");
            foreach (Word word in bowSpace.Words)
            {
                if (j != migraineIdx && j != magnesiumIdx)
                {
                    //Console.WriteLine("{0} {1}", word.MostFrequentForm, j);
                    // compute bisociativity between the word and "migraine"
                    double migraineBisoc = bisoc.GetSimilarity(mat[j], mat[migraineIdx]);
                    // compute bisociativity between the word and "magnesium"
                    double magnesiumBisoc = bisoc.GetSimilarity(mat[j], mat[magnesiumIdx]);
                    if (migraineBisoc > 0 && magnesiumBisoc > 0 && !stopWords.Contains(word.MostFrequentForm))
                    {
                        writer.WriteLine("{0},{1},{2},{3},{4},{5}",
                            word.MostFrequentForm,
                            stopWords.Contains(word.MostFrequentForm) ? "yes" : "",
                            goldenStandard.Contains(bowSpace.Stemmer.GetStem(word.MostFrequentForm)) ? "yes" : "",
                            migraineBisoc,
                            magnesiumBisoc,
                            Math.Min(migraineBisoc, magnesiumBisoc));
                    }
                }
                j++;
            }
            writer.Close();
        }

        private static void Main(string[] args)
        {
            // read stop words and golden standard
            Dictionary<string, ArrayList<string>> stopWords = new Dictionary<string, ArrayList<string>>();
            string[] stopWordList = File.ReadAllLines("..\\..\\data\\stopwords.txt");
            PorterStemmer stemmer = new PorterStemmer();
            foreach (string stopWord in stopWordList)
            {
                string stem = stemmer.GetStem(stopWord);
                if (stopWords.ContainsKey(stem))
                {
                    stopWords[stem].Add(stopWord);
                }
                else
                {
                    stopWords.Add(stem, new ArrayList<string>(new string[] { stopWord }));
                }
            }
            string goldenStandard = File.ReadAllText("..\\..\\data\\goldenstandard.txt");
            RegexTokenizer tokenizer = new RegexTokenizer();
            tokenizer.IgnoreUnknownTokens = true;
            tokenizer.TokenRegex = @"\w(\-?\w)+";
            tokenizer.Text = goldenStandard;
            Set<string> goldenStandardSet = new Set<string>();
            foreach (string token in tokenizer)
            {
                string stem = stemmer.GetStem(token);
                //if (stopWords.ContainsKey(stem)) { Console.WriteLine(stopWords[stem]); }
                stopWords.Remove(stem);
                goldenStandardSet.Add(stem);
            }
            Set<string> stopWordSet = new Set<string>();
            foreach (KeyValuePair<string, ArrayList<string>> item in stopWords)
            {
                stopWordSet.AddRange(item.Value);
            }
            // preprocess documents
            Console.WriteLine("-> Processing documents ...");
            string[] labeledDocs = File.ReadAllLines(@"..\..\data\MigraineMagnesium_LabeledTitles.txt");
            string[] labels = new string[labeledDocs.Length];
            string[] docs = new string[labeledDocs.Length];
            string[] migraineKeywords = new string[labeledDocs.Length];
            string[] magnesiumKeywords = new string[labeledDocs.Length];
            int i = 0;
            foreach (string labeledDoc in labeledDocs)
            {
                int j = labeledDoc.IndexOf('\t');
                labels[i] = labeledDoc.Substring(0, j);
                docs[i] = HttpUtility.HtmlDecode(labeledDoc.Substring(j + 1, labeledDoc.Length - (j + 1)));
                i++;
            }
            BowSpace bowSpace = new BowSpace();
            //Set<string> tmp = new Set<string>(StopWords.EnglishStopWords);
            //tmp.AddRange(stopWordSet);
            //bowSpace.StopWords = tmp;
            bowSpace.StopWords = StopWords.EnglishStopWords;
            bowSpace.Stemmer = stemmer;
            bowSpace.WordWeightType = WordWeightType.TfIdf;
            bowSpace.MinWordFreq = 1;
            bowSpace.MaxNGramLen = 3;
            bowSpace.CutLowWeightsPerc = 0;
            bowSpace.Tokenizer = tokenizer;
            bowSpace.Initialize(docs);
            // compute bisociativity for terms
            ComputeBisociativity(docs, tokenizer, stemmer, stopWordSet, goldenStandardSet);
            // determine misclassified documents
            LabeledDataset<string, SparseVector<double>.ReadOnly> dataset = new LabeledDataset<string, SparseVector<double>.ReadOnly>();
            for (i = 0; i < bowSpace.BowVectors.Count; i++)
            {
                dataset.Add(labels[i], bowSpace.BowVectors[i]);
            }
            BatchUpdateCentroidClassifier<string> classifier = new BatchUpdateCentroidClassifier<string>();
            classifier.Iterations = 0;
            Console.WriteLine("-> Training classifier ...");
            classifier.Train(dataset);
            Console.WriteLine("-> Classifying documents ...");
            i = 0;
            //StreamWriter writer = new StreamWriter("..\\..\\data\\misclassified.txt");
            foreach (SparseVector<double>.ReadOnly vec in bowSpace.BowVectors)
            {
                Prediction<string> prediction = classifier.Predict(vec);
                if (prediction.BestClassLabel != labels[i])
                {
                    labels[i] += "*";
                    //writer.WriteLine(docs[i]);
                }
                i++;
            }
            //writer.Close();
            Console.WriteLine("-> Computing opposite keywords ...");
            ArrayList<SparseVector<double>> centroids = classifier.GetCentroids(new string[] { "MIGRAINE", "MAGNESIUM" });
            // compute potential b-terms
            SparseVector<double> bTerms = GetProdVec(centroids[0], centroids[1]);
            Console.WriteLine(TermsToString2(bowSpace.GetKeywords(bTerms, int.MaxValue), stopWordSet, goldenStandardSet.Clone(), stemmer));
            //TermsToStringCsv(bowSpace.GetKeywords(bTerms), stopWordSet, goldenStandardSet.Clone(), stemmer, "..\\..\\data\\swansonTerms.csv");
            // compute opposite keywords
            for (i = 0; i < docs.Length; i++)
            {
                SparseVector<double> migraineProdVec = GetProdVec(bowSpace.BowVectors[i], centroids[0]);
                SparseVector<double> magnesiumProdVec = GetProdVec(bowSpace.BowVectors[i], centroids[1]);
                //migraineKeywords[i] = TermsToString(bowSpace.GetKeywords(migraineProdVec, 5), stopWordSet, goldenStandardSet, stemmer) + (migraineProdVec.Count > 5 ? " ..." : "");
                //magnesiumKeywords[i] = TermsToString(bowSpace.GetKeywords(magnesiumProdVec, 5), stopWordSet, goldenStandardSet, stemmer) + (magnesiumProdVec.Count > 5 ? " ..." : ""); 
                migraineKeywords[i] = TermsToString(bowSpace.GetKeywords(migraineProdVec, int.MaxValue), stopWordSet, goldenStandardSet, stemmer);
                magnesiumKeywords[i] = TermsToString(bowSpace.GetKeywords(magnesiumProdVec, int.MaxValue), stopWordSet, goldenStandardSet, stemmer);
            }
            // compute layout
            Console.WriteLine("-> Computing layout ...");
            SemanticSpaceLayout layout = new SemanticSpaceLayout(bowSpace);
            layout.NeighborhoodSize = 30;
            layout.SimThresh = 0;
            Latino.Visualization.LayoutSettings settings = new Latino.Visualization.LayoutSettings(1024, 768);
            settings.BoundsType = LayoutBoundsType.Circular;
            settings.FitToBounds = false;
            settings.AdjustmentType = LayoutAdjustmentType.Soft;
            Vector2D[] coords = layout.ComputeLayout(settings);
            //StreamWriter writer = new StreamWriter("..\\..\\data\\layoutMigraine.txt");
            //StreamWriter writer2 = new StreamWriter("..\\..\\data\\layoutMagnesium.txt");
            //i = 0;
            //foreach (Vector2D pt in coords)
            //{                
            //    if (labels[i] == "MIGRAINE")
            //    {
            //        writer.WriteLine("{0}\t{1}", pt.X, pt.Y);
            //    }
            //    else
            //    {
            //        writer2.WriteLine("{0}\t{1}", pt.X, pt.Y);
            //    }
            //    i++;
            //}
            //writer.Close();
            //writer2.Close();
            // visualize layout
            Console.WriteLine("-> Preparing visualization ...");
            MainWnd mainWnd = new MainWnd();
            DrawableGroup vis = new DrawableGroup();
            Color redColor = Color.FromArgb(64, 255, 0, 0);
            Color greenColor = Color.FromArgb(64, 0, 0, 255);
            Brush redBrush = new SolidBrush(redColor);
            Brush blueBrush = new SolidBrush(greenColor);
            i = 0;
            foreach (Vector2D pt in coords)
            {
                Document ptVis = new Document((float)pt.X, (float)pt.Y, 3, 3);
                ptVis.Idx = i;
                ptVis.Text = docs[i];
                ptVis.Label = labels[i];
                ptVis.Pen = Pens.Transparent;
                if (labels[i].StartsWith("MIGRAINE"))
                {
                    ptVis.Brush = redBrush;
                    if (labels[i].EndsWith("*")) { ptVis.Pen = Pens.Red; }
                    ptVis.OppositeKeywords = magnesiumKeywords[i];
                }
                else
                {
                    ptVis.Brush = blueBrush;
                    if (labels[i].EndsWith("*")) { ptVis.Pen = Pens.Blue; }
                    ptVis.OppositeKeywords = migraineKeywords[i];
                }
                vis.DrawableObjects.Add(ptVis);
                i++;
            }
            mainWnd.DrawableObjectViewer.DrawableObject = vis;
            Console.WriteLine("-> Done.");
            mainWnd.ShowDialog();
        }
    }
}
