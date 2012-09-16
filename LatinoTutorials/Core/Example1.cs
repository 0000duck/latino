using System;
using Latino;

namespace Latino.Tutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // create an ArrayList of KeyDat<double, int>
            ArrayList<KeyDat<double, int>> list = new ArrayList<KeyDat<double, int>>();
            // populate it
            list.AddRange(new KeyDat<double, int>[] {
                new KeyDat<double, int>(0.1, 1),
                new KeyDat<double, int>(0.3, 3),
                new KeyDat<double, int>(0.2, 2)});
            // sort it descendingly
            list.Sort(DescSort<KeyDat<double, int>>.Instance);
            // output it to the console
            Console.WriteLine(list); // says: ( ( 0.3 3 ) ( 0.2 2 ) ( 0.1 1 ) )
            // create a read-only wrapper
            ArrayList<KeyDat<double, int>>.ReadOnly listReadOnly
                = new ArrayList<KeyDat<double, int>>.ReadOnly(list);
            //listReadOnly.Add(new KeyDat<double, int>(0.4, 4)); // this is not possible
            // output the read-only list to the console
            Console.WriteLine(listReadOnly); // says: ( ( 0.3 3 ) ( 0.2 2 ) ( 0.1 1 ) )
            // create a writable copy
            ArrayList<KeyDat<double, int>> listCopy = listReadOnly.GetWritableCopy();
            // modify the copy
            listCopy.Add(new KeyDat<double, int>(0.4, 4));
            // output the original list to the console to show that it was not changed
            Console.WriteLine(list); // says: ( ( 0.3 3 ) ( 0.2 2 ) ( 0.1 1 ) )
            // use Inner to modify the enwrapped data structure
            listReadOnly.Inner.Add(new KeyDat<double, int>(0.4, 4));
            // output the original list to the console to show that it was changed
            Console.WriteLine(list); // says: ( ( 0.3 3 ) ( 0.2 2 ) ( 0.1 1 ) ( 0.4 4 ) )
        }
    }
}