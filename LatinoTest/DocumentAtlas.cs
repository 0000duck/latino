using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Latino.Model;
using Latino.TextMining;
using Latino.Visualization;

namespace Latino
{
    static class DocumentAtlas
    {
        public static void Exec()
        {
            //IncrementalBowSpace bow_space2 = new IncrementalBowSpace();
            //bow_space2.StopWords = StopWords.EnglishStopWords;
            //bow_space2.Stemmer = new PorterStemmer();
            //bow_space2.WordWeightType = WordWeightType.TfIdf;
            //bow_space2.MinWordFreq = 5;
            //RegexTokenizer tokenizer2 = new RegexTokenizer();
            //tokenizer2.IgnoreUnknownTokens = true;
            //bow_space2.Tokenizer = tokenizer2;
            //bow_space2.Initialize(docs);

            //StreamWriter writer = new StreamWriter("c:\\stats1.txt");
            //bow_space2.OutputStats(writer);
            //writer.Close();

            //SparseVectorDataset<byte> ds = new SparseVectorDataset<byte>();
            //foreach (SparseVector<double> vec in bow_space2.GetBowVectors())
            //{
            //    ds.Add(0, vec);
            //}
            //SparseMatrix<double> mtx1 = ds.GetDotProductSimilarity();

            //int k = 1;
            //foreach (string doc in docs)
            //{
            //    Console.Write("{0}\r", k++);
            //    bow_space2.Dequeue(1);
            //    bow_space2.Enqueue(new string[] { doc });
            //}

            //writer = new StreamWriter("c:\\stats2.txt");
            //bow_space2.OutputStats(writer);
            //writer.Close();

            //ds = new SparseVectorDataset<byte>();
            //foreach (SparseVector<double> vec in bow_space2.GetBowVectors())
            //{
            //    ds.Add(0, vec);
            //}
            //SparseMatrix<double> mtx2 = ds.GetDotProductSimilarity();
            //Console.WriteLine("Comparing ...");
            //foreach (IdxDat<SparseVector<double>> row in mtx1)
            //{
            //    foreach (IdxDat<double> item in row.Dat)
            //    {
            //        if (Math.Abs(item.Dat - mtx2[row.Idx, item.Idx]) > 0.00001)
            //        {
            //            Console.WriteLine("{0} {1}", item.Dat, mtx2[row.Idx, item.Idx]);
            //        }
            //    }
            //}

            // load documents
            Utils.VerboseLine("Loading documents ...");
            string[] docs = File.ReadAllLines("C:\\newwork\\testclustering\\data\\yahoofinance.txt");
            BowSpace bow_space = new BowSpace();
            bow_space.StopWords = StopWords.EnglishStopWords;
            bow_space.Stemmer = new PorterStemmer();
            bow_space.WordWeightType = WordWeightType.TfIdf;
            RegexTokenizer tokenizer = new RegexTokenizer();
            tokenizer.IgnoreUnknownTokens = true;
            bow_space.Tokenizer = tokenizer;
            bow_space.Initialize(docs);
            // compute layout
            SparseVectorDataset<byte> dataset = new SparseVectorDataset<byte>();
            foreach (SparseVector<double>.ReadOnly bow in bow_space.BowVectors)
            {
                dataset.Add(0, bow);
            }
            SemanticSpace<byte> sem_spc = new SemanticSpace<byte>(dataset);
            Vector2D[] coords = sem_spc.ComputeLayout();
            // output coordinates
            StreamWriter tsv_writer = new StreamWriter("c:\\layout.tsv");
            for (int i = 0; i < coords.Length; i++)
            {
                tsv_writer.WriteLine("{0}\t{1}", coords[i].X, coords[i].Y);
            }
            tsv_writer.Close();
            //// get document names
            //int k = 0;
            //ArrayList<Pair<string, Vector2D>> layout = new ArrayList<Pair<string, Vector2D>>();
            //foreach (string doc in docs)
            //{
            //    string[] doc_info = doc.Split(' ');
            //    layout.Add(new Pair<string, Vector2D>(doc_info[0], coords[k++]));
            //}
            //Console.WriteLine(coords.Length);
            //Console.WriteLine(layout.Count);
            //StreamWriter writer = new StreamWriter("c:\\vid_coords.txt");
            //foreach (Pair<string, Vector2D> doc_pos in layout)
            //{
            //    writer.WriteLine("{0}\t{1}\t{2}", doc_pos.First, doc_pos.Second.X, doc_pos.Second.Y);
            //}
            //writer.Close();
        }
    }
}
