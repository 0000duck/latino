using System;
using Latino;

namespace Latino.Tutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // create and populate an ArrayList of Ref<int>
            ArrayList<Ref<int>> array = new ArrayList<Ref<int>>(new Ref<int>[] { 
                new Ref<int>(1), 
                new Ref<int>(2), 
                new Ref<int>(3)});
            // create a shallow clone
            ArrayList<Ref<int>> arrayClone = array.Clone();
            // change the original array
            array[0].Val = 4;
            // output the two arrays to the console
            Console.WriteLine(array); // says: ( 4 2 3 )
            Console.WriteLine(arrayClone); // says: ( 4 2 3 )
            // restore the original array
            array[0].Val = 1;
            // create a deep clone
            ArrayList<Ref<int>> deepClone = array.DeepClone();
            // change the original array
            array[0].Val = 4;
            // output the two arrays to the console
            Console.WriteLine(array); // says: ( 4 2 3 )
            Console.WriteLine(deepClone); // says: ( 1 2 3 )
        }
    }
}
