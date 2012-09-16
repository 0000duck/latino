using System;
using Latino;

namespace Latino.Tutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // create two instances of Set<int> and populate them
            Set<int> set = new Set<int>(new int[] { 1, 2, 3 });
            Set<int> otherSet = new Set<int>(new int[] { 1, 2, 3 });
            // output them to the console
            Console.WriteLine(set); // says: { 1 2 3 }
            Console.WriteLine(otherSet); // says: { 1 2 3 }
            // compare them by reference
            Console.WriteLine(set == otherSet); // says: False
            // compare them by content
            Console.WriteLine(set.ContentEquals(otherSet)); // says: True
        }
    }
}