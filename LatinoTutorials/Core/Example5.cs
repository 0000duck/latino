using System;
using Latino;

namespace Latino.Tutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // create a Set<int> 
            Set<int> set = new Set<int>(new int[] { 1, 2, 3 });
            // create another Set<int> 
            Set<int> otherSet = new Set<int>(new int[] { 2, 3, 4 });
            // compute the union and output it to the console
            Console.WriteLine(Set<int>.Union(set, otherSet)); // says: { 1 2 3 4 }
            // compute the intersection and output it to the console
            Console.WriteLine(Set<int>.Intersection(set, otherSet)); // says: { 2 3 }
            // compute the difference and output it to the console
            Console.WriteLine(Set<int>.Difference(set, otherSet)); // says: { 1 }
            // compute the Jaccard similarity
            Console.WriteLine(Set<int>.JaccardSimilarity(set, otherSet)); // says: 0.5
        }
    }
}
