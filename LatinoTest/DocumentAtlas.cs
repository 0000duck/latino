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
            const int k_nn = 10;
            const int k_clust = 50;
            Random random = new Random(1);
            const double thresh = 0.005;
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
            SparseVectorDataset<byte> dataset = new SparseVectorDataset<byte>();
            foreach (SparseVector<double>.ReadOnly bow_vec in bow_space.BowVectors)
            {
                dataset.Add(0, bow_vec);
            }
            // clustering 
            Utils.VerboseLine("Clustering ...");
            KMeansFast<byte> k_means = new KMeansFast<byte>(k_clust);
            k_means.Eps = 0.01;
            k_means.Random = random;
            k_means.Trials = 1;
            ClusteringResult clustering = k_means.Cluster(dataset);
            // determine reference points
            Utils.VerboseLine("Computing reference points ...");
            SparseVectorDataset<byte> ds_cent = new SparseVectorDataset<byte>();
            foreach (Cluster cluster in clustering.Roots)
            {
                SparseVector<double> centroid = cluster.ComputeCentroid(dataset, CentroidType.NrmL2);
                ds_cent.Add(0, centroid); // dataset of centroids
                dataset.Add(0, centroid); // add centroids to the main dataset
            }
            // position reference points
            Utils.VerboseLine("Positioning reference points ...");
            SparseMatrix<double> sim_mtx = ds_cent.GetDotProductSimilarity(thresh, /*full_matrix=*/false);
            StressMajorization sm = new StressMajorization(ds_cent.Count, new DistFunc(sim_mtx));
            sm.Random = random;
            Vector2D[] pos_array = sm.ComputeLayout();
            #region output
            //Console.WriteLine(new ArrayList<VectorD>(pos_array));
            //StreamWriter tsv_writer = new StreamWriter("c:\\layout.tsv");
            //foreach (Vector2D pt in pos_array)
            //{
            //    tsv_writer.WriteLine("{0}\t{1}", pt.X, pt.Y);
            //}
            //tsv_writer.Close();
            #endregion
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
                double wgt = 1.0 / (double)count;
                SparseVector<double> eq = new SparseVector<double>();
                for (int i = 0; i < count; i++)
                {
                    KeyDat<double, int> item = knn[i];
                    eq.InnerIdx.Add(item.Dat);
                    eq.InnerDat.Add(-wgt);
                }
                eq.InnerIdx.Sort(); // *** sort only indices
                //eq.Sort();
                eq[sim_mtx_row.Idx] = 1;
                lsqr_ds.Add(0, eq);
            }
            Console.WriteLine(lsqr_ds.Count);
            Console.WriteLine(dataset.Count);
            for (int i = dataset.Count - k_clust, j = 0; i < dataset.Count; i++, j++)
            {
                SparseVector<double> eq = new SparseVector<double>(new IdxDat<double>[]{ new IdxDat<double>(i, 1) });
                lsqr_ds.Add(pos_array[j].X, eq);
            }
            LSqrModel lsqr = new LSqrModel();
            lsqr.Train(lsqr_ds);
            for (int i = dataset.Count, j = 0; i < lsqr_ds.Count; i++, j++)
            {
                //lsqr_ds[i] = new LabeledExample<double, SparseVector<double>.ReadOnly>(pos_array[j].Y, lsqr_ds[i].Example);
            }
            lsqr.Train(lsqr_ds);
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
