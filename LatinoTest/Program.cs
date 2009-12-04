using System.IO;
using Latino;
using Latino.TextMining;
using Latino.Visualization;
using System;

namespace LatinoTest
{
    class Program
    {
        static void Main(string[] args)
        {
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
            SemanticSpaceLayout sem_spc = new SemanticSpaceLayout(bow_space);
            Vector2D[] coords = sem_spc.ComputeLayout();
            // build spatial index
            //Utils.VerboseLine("Building spatial index ...");
            //SpatialIndex2D spat_idx = new SpatialIndex2D();
            //spat_idx.BuildIndex(coords);
            //spat_idx.InsertPoint(9000, new Vector2D(1000, 1000));
            //ArrayList<IdxDat<Vector2D>> points = spat_idx.GetPoints(new Vector2D(0.5, 0.5), 0.1);
            //Utils.VerboseLine("Number of retrieved points: {0}.", points.Count);

            ArrayList<Vector2D> tmp = new ArrayList<Vector2D>(coords);
            tmp.Shuffle();
            //tmp.RemoveRange(1000, tmp.Count - 1000);

            // compute elevation
            StreamWriter writer = new StreamWriter("c:\\elev.txt");
            LayoutSettings ls = new LayoutSettings(800, 600);
            ls.AdjustmentType = LayoutAdjustmentType.Soft;
            ls.StdDevMult = 2;
            ls.FitToBounds = true;
            ls.MarginVert = 50;
            ls.MarginHoriz = 50;
            double[,] z_mtx = VisualizationUtils.ComputeLayoutElevation(tmp, ls, 150, 200);
            VisualizationUtils.__DrawElevation__(tmp, ls, 300, 400).Save("c:\\elev.bmp");
            for (int row = 0; row < z_mtx.GetLength(0); row++)
            {
                for (int col = 0; col < z_mtx.GetLength(1); col++)
                {
                    writer.Write("{0}\t", z_mtx[row, col]);
                }
                writer.WriteLine();
            }
            writer.Close();
            
            // output coordinates
            StreamWriter tsv_writer = new StreamWriter("c:\\layout.tsv");
            for (int i = 0; i < coords.Length; i++)
            {
                //if (i < points.Count)
                //{
                //    tsv_writer.WriteLine("{0}\t{1}\t{2}\t{3}", coords[i].X, coords[i].Y, points[i].Dat.X, points[i].Dat.Y);
                //}
                //else
                {
                    tsv_writer.WriteLine("{0}\t{1}", coords[i].X, coords[i].Y);
                }
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
