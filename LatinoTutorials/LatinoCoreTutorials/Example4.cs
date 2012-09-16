using System;
using System.IO;
using Latino;

namespace Latino.Tutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // create an ArrayList<BinaryVector>
            ArrayList<BinaryVector> array = new ArrayList<BinaryVector>();
            // populate it
            array.AddRange(new BinaryVector[] {
                new BinaryVector(new int[] { 1, 3, 5 }),
                new BinaryVector(new int[] { 2, 4, 6 })});
            // output it to the console
            Console.WriteLine(array); // says: ( ( 1 3 5 ) ( 2 4 6 ) )
            // serialize it to a file
            BinarySerializer fileWriter = new BinarySerializer("array.bin", FileMode.Create);
            array.Save(fileWriter);
            fileWriter.Close();
            // read it from the file
            BinarySerializer fileReader = new BinarySerializer("array.bin", FileMode.Open);
            ArrayList<BinaryVector> otherArray = new ArrayList<BinaryVector>(fileReader);
            fileReader.Close();
            // output it to the console
            Console.WriteLine(otherArray); // says: ( ( 1 3 5 ) ( 2 4 6 ) )
            // compare it to the original array
            Console.WriteLine(array == otherArray); // says: False
            Console.WriteLine(array.ContentEquals(otherArray)); // says: True
        }
    }
}