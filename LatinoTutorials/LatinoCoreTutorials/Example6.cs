using System;
using Latino;

namespace Latino.Tutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // create a SparseVector<double>
            SparseVector<double> vec = new SparseVector<double>(new IdxDat<double>[] {             
                new IdxDat<double>(0, 0.0),
                new IdxDat<double>(2, 0.2),
                new IdxDat<double>(4, 0.4),
                new IdxDat<double>(6, 0.6)});
            // output it to the console 
            Console.WriteLine(vec.ToString()); // says: ( ( 0 0 ) ( 2 0.2 ) ( 4 0.4 ) ( 6 0.6 ) )
            // traverse it (says: (0,0) (2,0.2) (4,0.4) (6,0.6))
            foreach (IdxDat<double> item in vec)
            {
                Console.Write("({0},{1}) ", item.Idx, item.Dat);
            }
            Console.WriteLine();
            // add an element
            vec[1] = 0.1;
            // change a value
            vec[0] = 42;
            // remove an element
            vec.RemoveAt(4);
            // output the vector to the console 
            Console.WriteLine(vec.ToString()); // says: ( ( 0 42 ) ( 1 0.1 ) ( 2 0.2 ) ( 6 0.6 ) )
            // get the number of non-empty elements
            Console.WriteLine(vec.Count); // says: 4
            // get the number of dimensions
            Console.WriteLine(vec.LastNonEmptyIndex + 1); // says: 7
        }
    }
}