using System;
using Latino;
using Latino.Model;

namespace Latino.Tutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // load datasets
            LabeledDataset<int, SparseVector<double>> trainSet = ModelUtils.LoadDataset(@"..\..\Datasets\train1.dat");
            LabeledDataset<int, SparseVector<double>> testSet = ModelUtils.LoadDataset(@"..\..\Datasets\test1.dat");
            // train a nearest centroid classifier            
            NearestCentroidClassifier<int> classifier = new NearestCentroidClassifier<int>();
            classifier.Similarity = CosineSimilarity.Instance;
            classifier.NormalizeCentroids = false;
            classifier.Train(trainSet);
            // test the classifier
            int correct = 0;
            int all = 0;
            foreach (LabeledExample<int, SparseVector<double>> labeledExample in testSet)
            {
                if (labeledExample.Example.Count != 0)
                {
                    Prediction<int> prediction = classifier.Predict(labeledExample.Example);
                    if (prediction.BestClassLabel == labeledExample.Label) { correct++; }
                    all++;
                }
            }
            // output the result
            Console.WriteLine("Correctly classified: {0} of {1} ({2:0.00}%)", correct, all, (double)correct / (double)all * 100.0);
        }
    }
}