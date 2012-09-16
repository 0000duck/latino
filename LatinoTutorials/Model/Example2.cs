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
            LabeledDataset<int, SparseVector<double>> trainSet = ModelUtils.LoadDataset(@"..\..\Datasets\Example1\train.dat");
            LabeledDataset<int, SparseVector<double>> testSet = ModelUtils.LoadDataset(@"..\..\Datasets\Example1\test.dat");
            // normalize the feature vectors
            foreach (LabeledExample<int, SparseVector<double>> labeledExample in trainSet) { ModelUtils.TryNrmVecL2(labeledExample.Example); }
            foreach (LabeledExample<int, SparseVector<double>> labeledExample in testSet) { ModelUtils.TryNrmVecL2(labeledExample.Example); }
            // train a centroid classifier            
            BatchUpdatedCentroidClassifier<int> classifier = new BatchUpdatedCentroidClassifier<int>();
            classifier.Logger = Logger.GetInstanceLogger("Latino.Model.BatchUpdateCentroidClassifier");
            classifier.Iterations = 20;
            classifier.PositiveValuesOnly = true;
            classifier.Damping = 0.8;
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