import clr
from Latino import *
from Latino.Model import *
 
# load datasets
trainSet = ModelUtils.LoadDataset("..\\..\\Datasets\\train1.dat")
testSet = ModelUtils.LoadDataset("..\\..\\Datasets\\test1.dat")

# train a centroid classifier
classifier = CentroidClassifier[int]()
classifier.Similarity = CosineSimilarity.Instance
classifier.NormalizeCentroids = False
classifier.Train(trainSet)

# test the classifier
correct = 0
all = 0
for labeledExample in testSet:
  if labeledExample.Example.Count != 0:
    prediction = classifier.Predict(labeledExample.Example)
    if prediction.BestClassLabel == labeledExample.Label: correct += 1
    all += 1

# output the result
print "Correctly classified: {0} of {1} ({2:0.2f}%)".format(correct, all, float(correct) / float(all) * 100.0)