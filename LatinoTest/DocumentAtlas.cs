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
            const int k_nn = 50;
            const int k_clust = 100;            
            const double thresh = 0.005;
            const double k_means_eps = 0.01;
            Random random = new Random(1);
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

            IncrementalBowSpace bow_space2 = new IncrementalBowSpace();
            bow_space2.StopWords = StopWords.EnglishStopWords;
            bow_space2.Stemmer = new PorterStemmer();
            bow_space2.WordWeightType = WordWeightType.TfIdf;
            RegexTokenizer tokenizer2 = new RegexTokenizer();
            tokenizer2.IgnoreUnknownTokens = true;
            bow_space2.Tokenizer = tokenizer2;
            bow_space2.Initialize(docs);

            //ArrayList<SparseVector<double>> bow_vectors = bow_space2.GetBowVectors();
            //for (int i = 0; i < bow_space.BowVectors.Count; i++)
            //{
            //    Console.WriteLine(bow_space.BowVectors[i].ContentEquals(bow_vectors[i]));
            //}

            bow_space2.Dequeue(500);

            return;

            SparseVectorDataset<byte> dataset = new SparseVectorDataset<byte>();
            foreach (SparseVector<double>.ReadOnly bow_vec in bow_space.BowVectors)
            {
                dataset.Add(0, bow_vec);
            }
            // clustering 
            Utils.VerboseLine("Clustering ...");
            KMeansFast<byte> k_means = new KMeansFast<byte>(k_clust);
            k_means.Eps = k_means_eps;
            k_means.Random = random;
            k_means.Trials = 1;
            ClusteringResult clustering = k_means.Cluster(dataset);
            // determine reference instances
            Utils.VerboseLine("Computing reference instances ...");
            SparseVectorDataset<byte> ds_ref_inst = new SparseVectorDataset<byte>();
            foreach (Cluster cluster in clustering.Roots)
            {
                SparseVector<double> centroid = cluster.ComputeCentroid(dataset, CentroidType.NrmL2);
                ds_ref_inst.Add(0, centroid); // dataset of reference instances
                dataset.Add(0, centroid); // add centroids to the main dataset
            }
            // take random instances from each cluster
            // ...
            // position reference instances
            Utils.VerboseLine("Positioning reference instances ...");
            SparseMatrix<double> sim_mtx = ds_ref_inst.GetDotProductSimilarity(thresh, /*full_matrix=*/false);
            StressMajorization sm = new StressMajorization(ds_ref_inst.Count, new DistFunc(sim_mtx));
            sm.Random = random;
            Vector2D[] centr_pos = sm.ComputeLayout();
            // k-NN
            Utils.VerboseLine("Computing similarities ...");
            sim_mtx = dataset.GetDotProductSimilarity(thresh, /*full_matrix=*/true);
            Utils.VerboseLine("Constructing system of linear equations ...");
            SparseVectorDataset<double> lsqr_ds = new SparseVectorDataset<double>();
            foreach (IdxDat<SparseVector<double>> sim_mtx_row in sim_mtx)
            {                
                if (sim_mtx_row.Dat.Count == 0)
                {
                    Utils.VerboseLine("*** Warning: instance #{0} has no neighborhood.", sim_mtx_row.Idx);
                }
                ArrayList<KeyDat<double, int>> knn = new ArrayList<KeyDat<double, int>>(sim_mtx_row.Dat.Count);
                foreach (IdxDat<double> item in sim_mtx_row.Dat)
                {
                    if (item.Idx != sim_mtx_row.Idx)
                    {
                        knn.Add(new KeyDat<double, int>(item.Dat, item.Idx));
                    }
                }
                knn.Sort(new DescSort<KeyDat<double, int>>());
                int count = Math.Min(knn.Count, k_nn);
                SparseVector<double> eq = new SparseVector<double>();
#if ALT_EQ // *** this has no significant visual effect
                double wgt_sum = 0;
                for (int i = 0; i < count; i++)
                {
                    wgt_sum += knn[i].Key;
                }
                for (int i = 0; i < count; i++)
                {
                    eq.InnerIdx.Add(knn[i].Dat);
                    eq.InnerDat.Add(-knn[i].Key / wgt_sum);
                }                
                eq.Sort();                                
#else
                double wgt = 1.0 / (double)count;                
                for (int i = 0; i < count; i++)
                {
                    eq.InnerIdx.Add(knn[i].Dat);
                    eq.InnerDat.Add(-wgt);
                }
                eq.InnerIdx.Sort(); // *** sort only indices
                //eq.Sort();                                
#endif
                eq[sim_mtx_row.Idx] = 1;
                lsqr_ds.Add(0, eq);
            }
            Vector2D[] coords = new Vector2D[dataset.Count - k_clust];
            for (int i = dataset.Count - k_clust, j = 0; i < dataset.Count; i++, j++)
            {
                SparseVector<double> eq = new SparseVector<double>(new IdxDat<double>[]{ new IdxDat<double>(i, 1) });
                lsqr_ds.Add(centr_pos[j].X, eq);
            }
            LSqrModel lsqr = new LSqrModel();
            lsqr.Train(lsqr_ds);
            for (int i = 0; i < coords.Length; i++)
            {
                coords[i].X = lsqr.Solution[i];
            }                
            for (int i = lsqr_ds.Count - k_clust, j = 0; i < lsqr_ds.Count; i++, j++)
            {
                lsqr_ds[i].Label = centr_pos[j].Y;
            }
            lsqr.Train(lsqr_ds);
            for (int i = 0; i < coords.Length; i++)
            {
                coords[i].Y = lsqr.Solution[i];
            } 
            // output coordinates
            StreamWriter tsv_writer = new StreamWriter("c:\\layout.tsv");
            for (int i = 0; i < coords.Length; i++)
            {
                tsv_writer.Write("{0}\t{1}", coords[i].X, coords[i].Y);
                if (i < centr_pos.Length)
                {
                    tsv_writer.Write("\t{0}\t{1}", centr_pos[i].X, centr_pos[i].Y);
                }
                tsv_writer.WriteLine();
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

        private class DistFunc : IDistance<int>
        {
            private SparseMatrix<double>.ReadOnly m_sim_mtx;

            public DistFunc(SparseMatrix<double>.ReadOnly sim_mtx)
            {
                m_sim_mtx = sim_mtx;
            }

            public double GetDistance(int a, int b)
            {
                try
                {
                    if (a > b) { return 1.0 - m_sim_mtx[b, a]; }
                    else { return 1.0 - m_sim_mtx[a, b]; }
                }
                catch
                {
                    return 1;
                }
            }

            public void Save(BinarySerializer dummy)
            {
                throw new NotImplementedException();
            }
        }
    }
}
