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

        public Document(float x, float y, float r_x, float r_y) : base(x, y, r_x, r_y)
        {
        }
    }

    public static class Program
    {
        private static SparseVector<double> GetProdVec(SparseVector<double>.ReadOnly a, SparseVector<double>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            SparseVector<double> prod_vec = new SparseVector<double>();
            int i = 0, j = 0;
            int a_count = a.Count;
            int b_count = b.Count;
            if (a_count == 0 || b_count == 0) { return prod_vec; }
            ArrayList<int> a_idx = a.Inner.InnerIdx;
            ArrayList<double> a_dat = a.Inner.InnerDat;
            ArrayList<int> b_idx = b.Inner.InnerIdx;
            ArrayList<double> b_dat = b.Inner.InnerDat;
            int a_idx_i = a_idx[0];
            int b_idx_j = b_idx[0];
            while (true)
            {
                if (a_idx_i < b_idx_j)
                {
                    if (++i == a_count) { break; }
                    a_idx_i = a_idx[i];
                }
                else if (a_idx_i > b_idx_j)
                {
                    if (++j == b_count) { break; }
                    b_idx_j = b_idx[j];
                }
                else
                {
                    prod_vec.InnerIdx.Add(a_idx_i);
                    prod_vec.InnerDat.Add(a_dat[i] * b_dat[j]);
                    if (++i == a_count || ++j == b_count) { break; }
                    a_idx_i = a_idx[i];
                    b_idx_j = b_idx[j];
                }
            }
            return prod_vec;
        }

        private static bool IsAllStopWords(string term, Set<string>.ReadOnly stop_words)
        {
            string[] words = term.Split(' ');
            foreach (string word in words)
            {
                if (!stop_words.Contains(word)) { return false; }
            }
            return true;
        }

        private static bool ContainsGoldenStandardWord(string term, Set<string>.ReadOnly golden_standard, IStemmer stemmer)
        {
            string[] words = term.Split(' ');
            foreach (string word in words)
            {
                if (golden_standard.Contains(stemmer.GetStem(word))) { return true; }
            }
            return false;
        }

        private static bool ContainsGoldenStandardWord2(string term, Set<string> golden_standard, IStemmer stemmer)
        {
            string[] words = term.Split(' ');
            bool ret_val = false;
            foreach (string word in words)
            {
                string stem = stemmer.GetStem(word);
                if (golden_standard.Contains(stem))
                {
                    ret_val = true;
                    golden_standard.Remove(stem);
                }
            }
            return ret_val;
        }

        private static string TermsToString(IEnumerable<string> trems, Set<string>.ReadOnly stop_words, Set<string>.ReadOnly golden_standard, IStemmer stemmer)
        {
            StringBuilder str_builder = new StringBuilder();
            foreach (string term in trems)
            {
                if (IsAllStopWords(term, stop_words)) { str_builder.Append("(" + term + "), "); }
                else if (ContainsGoldenStandardWord(term, golden_standard, stemmer)) { str_builder.Append(term + "*, "); }
                else { str_builder.Append(term + ", "); }
            }
            return str_builder.ToString().TrimEnd(' ', ',');
        }

        private static void TermsToStringCsv(IEnumerable<KeyDat<double, string>> trems, Set<string>.ReadOnly stop_words, Set<string>.ReadOnly golden_standard, IStemmer stemmer, string file_name)
        {
            StreamWriter writer = new StreamWriter(file_name);
            StringBuilder str_builder = new StringBuilder();
            foreach (KeyDat<double, string> term in trems)
            {   
                if (true/*!IsAllStopWords(term.Dat, stop_words)*/) { str_builder.Append(string.Format("{0},", term.Key)); }
                if (IsAllStopWords(term.Dat, stop_words)) { str_builder.AppendLine(term.Dat + ",,STOP"); }
                else if (ContainsGoldenStandardWord(term.Dat, golden_standard, stemmer)) { str_builder.AppendLine(term.Dat + ",GOLD,"); }
                else { str_builder.AppendLine(term.Dat); }
            }
            writer.WriteLine(str_builder.ToString());
            writer.Close();
        }

        private static string TermsToString2(IEnumerable<string> trems, Set<string>.ReadOnly stop_words, Set<string> golden_standard, IStemmer stemmer)
        {
            StringBuilder str_builder = new StringBuilder();
            foreach (string term in trems)
            {
                if (IsAllStopWords(term, stop_words)) { /*str_builder.Append("(" + term + "), ");*/ }
                else if (ContainsGoldenStandardWord2(term, golden_standard, stemmer)) { str_builder.Append(term + "*, "); }
                else { str_builder.Append(term + ", "); }
            }
            return str_builder.ToString().TrimEnd(' ', ',');
        }

        private static int GetWordIdx(string word_stem, BowSpace bow_space)
        {
            int i = 0;
            foreach (Word word in bow_space.Words)
            {
                if (bow_space.Stemmer.GetStem(word.MostFrequentForm) == bow_space.Stemmer.GetStem(word_stem)) { return i; }
                i++;
            }
            return -1;
        }

        private static void ComputeBisociativity(IEnumerable<string> docs, ITokenizer tokenizer, IStemmer stemmer, Set<string>.ReadOnly stop_words, Set<string>.ReadOnly golden_standard)
        {
            Console.WriteLine("-> Computing bisociativity ...");
            // compute pure TF vectors
            BowSpace bow_space = new BowSpace();
            bow_space.StopWords = StopWords.EnglishStopWords;
            bow_space.Stemmer = stemmer;
            bow_space.WordWeightType = WordWeightType.TermFreq;
            bow_space.NormalizeVectors = false;
            bow_space.MinWordFreq = 1;
            bow_space.MaxNGramLen = 1;// 3;
            bow_space.CutLowWeightsPerc = 0;
            bow_space.Tokenizer = tokenizer;
            bow_space.Initialize(docs);
            // normalize TF vectors         
            foreach (SparseVector<double>.ReadOnly vec in bow_space.BowVectors)
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
            SparseMatrix<double> mat = ModelUtils.GetTransposedMatrix(bow_space);
            // compute bisociativity values
            StreamWriter writer = new StreamWriter("..\\..\\data\\swanson_bisoc.csv");
            Bisociativity bisoc = new Bisociativity(/*k=*/1);
            int migraine_idx = GetWordIdx("migraine", bow_space);
            int magnesium_idx = GetWordIdx("magnesium", bow_space);
            int j = 0;
            writer.WriteLine("Word,IsStopWord,IsGoldenStandardWord,Migraine,Magnesium,Min");
            foreach (Word word in bow_space.Words)
            {
                if (j != migraine_idx && j != magnesium_idx)
                {
                    //Console.WriteLine("{0} {1}", word.MostFrequentForm, j);
                    // compute bisociativity between the word and "migraine"
                    double migraine_bisoc = bisoc.GetSimilarity(mat[j], mat[migraine_idx]);
                    // compute bisociativity between the word and "magnesium"
                    double magnesium_bisoc = bisoc.GetSimilarity(mat[j], mat[magnesium_idx]);
                    if (migraine_bisoc > 0 && magnesium_bisoc > 0 && !stop_words.Contains(word.MostFrequentForm))
                    {
                        writer.WriteLine("{0},{1},{2},{3},{4},{5}",
                            word.MostFrequentForm,
                            stop_words.Contains(word.MostFrequentForm) ? "yes" : "",
                            golden_standard.Contains(bow_space.Stemmer.GetStem(word.MostFrequentForm)) ? "yes" : "",
                            migraine_bisoc,
                            magnesium_bisoc,
                            Math.Min(migraine_bisoc, magnesium_bisoc));
                    }
                }
                j++;
            }
            writer.Close();
        }

        private static void Main(string[] args)
        {
            // read stop words and golden standard
            Dictionary<string, ArrayList<string>> stop_words = new Dictionary<string, ArrayList<string>>();
            string[] stop_word_list = File.ReadAllLines("..\\..\\data\\stopwords.txt");
            PorterStemmer stemmer = new PorterStemmer();
            foreach (string stop_word in stop_word_list)
            {
                string stem = stemmer.GetStem(stop_word);
                if (stop_words.ContainsKey(stem))
                {
                    stop_words[stem].Add(stop_word);
                }
                else
                {
                    stop_words.Add(stem, new ArrayList<string>(new string[] { stop_word }));
                }
            }
            string golden_standard = File.ReadAllText("..\\..\\data\\goldenstandard.txt");
            RegexTokenizer tokenizer = new RegexTokenizer();
            tokenizer.IgnoreUnknownTokens = true;
            tokenizer.TokenRegex = @"\w(\-?\w)+";
            tokenizer.Text = golden_standard;
            Set<string> golden_standard_set = new Set<string>();
            foreach (string token in tokenizer)
            {
                string stem = stemmer.GetStem(token);
                //if (stop_words.ContainsKey(stem)) { Console.WriteLine(stop_words[stem]); }
                stop_words.Remove(stem);
                golden_standard_set.Add(stem);
            }
            Set<string> stop_word_set = new Set<string>();
            foreach (KeyValuePair<string, ArrayList<string>> item in stop_words)
            {
                stop_word_set.AddRange(item.Value);
            }
            // preprocess documents
            Console.WriteLine("-> Processing documents ...");
            string[] labeled_docs = File.ReadAllLines(@"..\..\data\MigraineMagnesium_LabeledTitles.txt");
            string[] labels = new string[labeled_docs.Length];
            string[] docs = new string[labeled_docs.Length];
            string[] migraine_keywords = new string[labeled_docs.Length];
            string[] magnesium_keywords = new string[labeled_docs.Length];
            int i = 0;
            foreach (string labeled_doc in labeled_docs)
            {
                int j = labeled_doc.IndexOf('\t');
                labels[i] = labeled_doc.Substring(0, j);
                docs[i] = HttpUtility.HtmlDecode(labeled_doc.Substring(j + 1, labeled_doc.Length - (j + 1)));
                i++;
            }
            BowSpace bow_space = new BowSpace();
            //Set<string> tmp = new Set<string>(StopWords.EnglishStopWords);
            //tmp.AddRange(stop_word_set);
            //bow_space.StopWords = tmp;
            bow_space.StopWords = StopWords.EnglishStopWords;
            bow_space.Stemmer = stemmer;
            bow_space.WordWeightType = WordWeightType.TfIdf;
            bow_space.MinWordFreq = 1;
            bow_space.MaxNGramLen = 3;
            bow_space.CutLowWeightsPerc = 0;
            bow_space.Tokenizer = tokenizer;
            bow_space.Initialize(docs);
            // compute bisociativity for terms
            ComputeBisociativity(docs, tokenizer, stemmer, stop_word_set, golden_standard_set);
            // determine misclassified documents
            LabeledDataset<string, SparseVector<double>.ReadOnly> dataset = new LabeledDataset<string, SparseVector<double>.ReadOnly>();
            for (i = 0; i < bow_space.BowVectors.Count; i++)
            {
                dataset.Add(labels[i], bow_space.BowVectors[i]);
            }
            BatchUpdateCentroidClassifier<string> classifier = new BatchUpdateCentroidClassifier<string>();
            classifier.Iterations = 0;
            Console.WriteLine("-> Training classifier ...");
            classifier.Train(dataset);
            Console.WriteLine("-> Classifying documents ...");
            i = 0;
            //StreamWriter writer = new StreamWriter("..\\..\\data\\misclassified.txt");
            foreach (SparseVector<double>.ReadOnly vec in bow_space.BowVectors)
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
            SparseVector<double> b_terms = GetProdVec(centroids[0], centroids[1]);
            Console.WriteLine(TermsToString2(bow_space.GetKeywords(b_terms, int.MaxValue), stop_word_set, golden_standard_set.Clone(), stemmer));
            //TermsToStringCsv(bow_space.GetKeywords(b_terms), stop_word_set, golden_standard_set.Clone(), stemmer, "..\\..\\data\\swanson_terms.csv");
            // compute opposite keywords
            for (i = 0; i < docs.Length; i++)
            {
                SparseVector<double> migraine_prod_vec = GetProdVec(bow_space.BowVectors[i], centroids[0]);
                SparseVector<double> magnesium_prod_vec = GetProdVec(bow_space.BowVectors[i], centroids[1]);
                //migraine_keywords[i] = TermsToString(bow_space.GetKeywords(migraine_prod_vec, 5), stop_word_set, golden_standard_set, stemmer) + (migraine_prod_vec.Count > 5 ? " ..." : "");
                //magnesium_keywords[i] = TermsToString(bow_space.GetKeywords(magnesium_prod_vec, 5), stop_word_set, golden_standard_set, stemmer) + (magnesium_prod_vec.Count > 5 ? " ..." : ""); 
                migraine_keywords[i] = TermsToString(bow_space.GetKeywords(migraine_prod_vec, int.MaxValue), stop_word_set, golden_standard_set, stemmer);
                magnesium_keywords[i] = TermsToString(bow_space.GetKeywords(magnesium_prod_vec, int.MaxValue), stop_word_set, golden_standard_set, stemmer);
            }
            // compute layout
            Console.WriteLine("-> Computing layout ...");
            SemanticSpaceLayout layout = new SemanticSpaceLayout(bow_space);
            layout.NeighborhoodSize = 30;
            layout.SimThresh = 0;
            Latino.Visualization.LayoutSettings settings = new Latino.Visualization.LayoutSettings(1024, 768);
            settings.BoundsType = LayoutBoundsType.Circular;
            settings.FitToBounds = false;
            settings.AdjustmentType = LayoutAdjustmentType.Soft;
            Vector2D[] coords = layout.ComputeLayout(settings);
            //StreamWriter writer = new StreamWriter("..\\..\\data\\layout_migraine.txt");
            //StreamWriter writer2 = new StreamWriter("..\\..\\data\\layout_magnesium.txt");
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
            MainWnd main_wnd = new MainWnd();
            DrawableGroup vis = new DrawableGroup();
            Color red_color = Color.FromArgb(64, 255, 0, 0);
            Color green_color = Color.FromArgb(64, 0, 0, 255);
            Brush red_brush = new SolidBrush(red_color);
            Brush blue_brush = new SolidBrush(green_color);
            i = 0;
            foreach (Vector2D pt in coords)
            {
                Document pt_vis = new Document((float)pt.X, (float)pt.Y, 3, 3);
                pt_vis.Idx = i;
                pt_vis.Text = docs[i];
                pt_vis.Label = labels[i];
                pt_vis.Pen = Pens.Transparent;
                if (labels[i].StartsWith("MIGRAINE"))
                {
                    pt_vis.Brush = red_brush;
                    if (labels[i].EndsWith("*")) { pt_vis.Pen = Pens.Red; }
                    pt_vis.OppositeKeywords = magnesium_keywords[i];
                }
                else
                {
                    pt_vis.Brush = blue_brush;
                    if (labels[i].EndsWith("*")) { pt_vis.Pen = Pens.Blue; }
                    pt_vis.OppositeKeywords = migraine_keywords[i];
                }
                vis.DrawableObjects.Add(pt_vis);
                i++;
            }
            main_wnd.DrawableObjectViewer.DrawableObject = vis;
            Console.WriteLine("-> Done.");
            main_wnd.ShowDialog();
        }
    }
}
