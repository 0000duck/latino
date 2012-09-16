using System;
using Latino;
using Latino.Model;

namespace Latino.Model.Tutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // load datasets
            LabeledDataset<int, SparseVector<double>> trainDataset = ModelUtils.LoadDataset(@"..\..\Datasets\Example1\train.dat");
            LabeledDataset<int, SparseVector<double>> testDataset = ModelUtils.LoadDataset(@"..\..\Datasets\Example1\test.dat");
            // train a centroid classifier            
            CentroidClassifier<int> classifier = new CentroidClassifier<int>();
            classifier.Similarity = CosineSimilarity.Instance;
            classifier.NormalizeCentroids = false;
            classifier.Train(trainDataset);
            // test the classifier
            int correct = 0;
            int all = 0;
            foreach (LabeledExample<int, SparseVector<double>> labeledExample in testDataset)
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