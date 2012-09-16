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
            for (int i = 0; i < vec.Count; i++)
            {
                Console.Write("({0},{1}) ", vec.GetIdxDirect(i), vec.GetDatDirect(i));
            }
            Console.WriteLine();
            // add an element
            vec.InnerIdx.Insert(1, 1); // !!! be careful !!!
            vec.InnerDat.Insert(1, 0.1); // !!! be careful !!!
            // output the vector to the console 
            Console.WriteLine(vec.ToString()); // says: ( ( 0 0 ) ( 1 0.1 ) ( 2 0.2 ) ( 4 0.4 ) ( 6 0.6 ) )
            // change a value
            vec.SetDirect(0, 42);
            // output the vector to the console 
            Console.WriteLine(vec.ToString()); // says: ( ( 0 42 ) ( 1 0.1 ) ( 2 0.2 ) ( 4 0.4 ) ( 6 0.6 ) )
            // remove an element
            vec.RemoveDirect(2);
            // output the vector to the console 
            Console.WriteLine(vec.ToString()); // says: ( ( 0 42 ) ( 1 0.1 ) ( 4 0.4 ) ( 6 0.6 ) )
        }
    }
}