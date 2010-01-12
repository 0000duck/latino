/*=====================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Tutorial1.cs
 *  Version:       1.0
 *  Desc:          Tutorial 1: Fundamental data structures
 *  Author:        Miha Grcar
 *  Created on:    Dec-2009
 *  Last modified: Dec-2009
 *  Revision:      Dec-2009
 *
 **********************************************************************/

using System;
using Latino;

namespace LatinoTutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // *** ArrayList ***
            Console.WriteLine("*** ArrayList ***");
            Console.WriteLine();
            // create an ArrayList 
            Console.WriteLine("Create an ArrayList ...");
            ArrayList<int> list = new ArrayList<int>(new int[] { 1, 2, 3 });            
            Console.WriteLine(list);
            // add more items
            Console.WriteLine("Add more items ...");
            list.AddRange(new int[] { 6, 5, 4 });
            Console.WriteLine(list);
            // sort descendingly
            Console.WriteLine("Sort descendingly ...");
            list.Sort(new DescSort<int>());
            Console.WriteLine(list);
            // shuffle 
            Console.WriteLine("Shuffle ...");
            list.Shuffle(new Random(1));
            Console.WriteLine(list);
            // convert to array of double
            Console.WriteLine("Convert to array of double ...");
            double[] array = list.ToArray<double>();
            Console.WriteLine(new ArrayList<double>(array));
            // convert to ArrayList of string
            Console.WriteLine("Convert to ArrayList of string ...");
            ArrayList<string> list2 = new ArrayList<string>(list.ToArray<string>());
            Console.WriteLine(list2);
            // get items
            Console.WriteLine("Get items ...");
            Console.WriteLine(list[0]);
            Console.WriteLine(list[1]);
            // set items
            Console.WriteLine("Set items ...");
            list[0] = 3;
            list[1] = 2;
            Console.WriteLine(list);
            // get length
            Console.WriteLine("Get length ...");
            Console.WriteLine(list.Count);
            Console.WriteLine();

            // *** Set ***
            Console.WriteLine("*** Set ***");
            Console.WriteLine();
            // create Set 
            Console.WriteLine("Create Set ...");
            Set<int> set = new Set<int>(new int[] { 1, 2, 3 });
            Console.WriteLine(set);
            // check for items
            Console.WriteLine("Check for items ...");
            Console.WriteLine(set.Contains(1));
            Console.WriteLine(set.Contains(4));
            // add more items (note the duplicate)
            Console.WriteLine("Add more items ...");
            set.AddRange(new int[] { 6, 5, 4, 3 });
            Console.WriteLine(set);
            // remove some items
            Console.WriteLine("Remove some items ...");
            set.RemoveRange(new int[] { 1, 3 });
            set.Remove(5);
            Console.WriteLine(set);
            // create another Set
            Console.WriteLine("Create another Set ...");
            Set<int> set2 = new Set<int>(new int[] { 1, 2, 3, 4, 5 });
            Console.WriteLine(set2);
            // compute union
            Console.WriteLine("Compute union ...");
            Console.WriteLine(Set<int>.Union(set, set2));
            // compute difference
            Console.WriteLine("Compute difference ...");
            Console.WriteLine(Set<int>.Difference(set, set2));
            // compute intersection
            Console.WriteLine("Compute intersection ...");
            Console.WriteLine(Set<int>.Intersection(set, set2));
            // compute Jaccard similarity
            Console.WriteLine("Compute Jaccard similarity ...");
            Console.WriteLine(Set<int>.JaccardSimilarity(set, set2));
            // convert to array
            Console.WriteLine("Convert to array ...");
            int[] array2 = set2.ToArray();
            Console.WriteLine(new ArrayList<int>(array2)); 
            // convert to Set of string
            Console.WriteLine("Convert to Set of string ...");
            Set<string> set3 = new Set<string>(set2.ToArray<string>());
            Console.WriteLine(set3);
            // get length
            Console.WriteLine("Get length ...");
            Console.WriteLine(set3.Count);
            Console.WriteLine();
            
            // *** BinaryVector ***
            Console.WriteLine("*** BinaryVector ***");
            Console.WriteLine();
            // create BinaryVector
            Console.WriteLine("Create BinaryVector ...");
            BinaryVector<char> bin_vec = new BinaryVector<char>(new char[] { 'a', 'b', 'c' });            
            Console.WriteLine(bin_vec);
            // check for items
            Console.WriteLine("Check for items ...");
            Console.WriteLine(bin_vec.Contains('a'));
            Console.WriteLine(bin_vec.Contains('d'));
            // add more items (note the duplicate)
            Console.WriteLine("Add more items ...");
            bin_vec.AddRange(new char[] { 'f', 'e', 'd', 'c' });
            Console.WriteLine(bin_vec);
            // remove some items
            Console.WriteLine("Remove some items ...");
            bin_vec.RemoveRange(new char[] { 'a', 'c' });
            bin_vec.Remove('e');
            Console.WriteLine(bin_vec);
            // convert to array
            Console.WriteLine("Convert to array ...");
            char[] array3 = bin_vec.ToArray();
            Console.WriteLine(new ArrayList<char>(array3));
            // convert to BinaryVector of string
            Console.WriteLine("Convert to BinaryVector of string ...");
            BinaryVector<string> bin_vec2 = new BinaryVector<string>(bin_vec.ToArray<string>());
            Console.WriteLine(bin_vec2);
            // get items
            Console.WriteLine("Get items ...");
            Console.WriteLine(bin_vec2[0]);
            Console.WriteLine(bin_vec2[1]);
            // get length
            Console.WriteLine("Get length ...");
            Console.WriteLine(bin_vec2.Count);            
            Console.WriteLine();

            // *** Pair ***
            Console.WriteLine("*** Pair ***");
            Console.WriteLine();
            // create Pair
            Console.WriteLine("Create Pair ...");
            Pair<int, string> pair = new Pair<int, string>(3, "dogs");
            Console.WriteLine(pair);
            // create another Pair
            Console.WriteLine("Create another Pair ...");
            Pair<int, string> pair2 = new Pair<int, string>(3, "cats");
            Console.WriteLine(pair2);
            // compare 
            Console.WriteLine("Compare ...");
            Console.WriteLine(pair == pair2);
            // make a change
            Console.WriteLine("Make a change ...");
            pair.Second = "cats";
            Console.WriteLine(pair);
            // compare again
            Console.WriteLine("Compare again ...");
            Console.WriteLine(pair == pair2);
            Console.WriteLine();

            // *** KeyDat ***
            Console.WriteLine("*** KeyDat ***");
            Console.WriteLine();
            // create KeyDat
            Console.WriteLine("Create KeyDat ...");
            KeyDat<int, string> key_dat = new KeyDat<int, string>(3, "dogs");
            Console.WriteLine(key_dat);
            // create another KeyDat
            Console.WriteLine("Create another KeyDat ...");
            KeyDat<int, string> key_dat2 = new KeyDat<int, string>(3, "cats");
            Console.WriteLine(key_dat2);
            // compare 
            Console.WriteLine("Compare ...");
            Console.WriteLine(key_dat == key_dat2);
            // make a change
            Console.WriteLine("Make a change ...");
            key_dat.Key = 4;
            Console.WriteLine(key_dat);
            // compare again
            Console.WriteLine("Compare again ...");
            Console.WriteLine(key_dat == key_dat2);
            Console.WriteLine(key_dat > key_dat2);
            Console.WriteLine();

            // *** IdxDat ***
            Console.WriteLine("*** IdxDat ***");
            Console.WriteLine();
            // create an IdxDat
            Console.WriteLine("Create an IdxDat ...");
            IdxDat<string> idx_dat = new IdxDat<string>(3, "dogs");
            Console.WriteLine(idx_dat);
            // create another IdxDat
            Console.WriteLine("Create another IdxDat ...");
            IdxDat<string> idx_dat2 = new IdxDat<string>(4, "cats");
            Console.WriteLine(idx_dat2);
            // compare 
            Console.WriteLine("Compare ...");
            Console.WriteLine(idx_dat == idx_dat2);
            // make a change
            //idx_dat.Idx = 4; // not possible to change index
            idx_dat.Dat = "cats";
            Console.WriteLine(idx_dat);
            // compare again
            Console.WriteLine("Compare again ...");
            Console.WriteLine(idx_dat == idx_dat2);
            Console.WriteLine(idx_dat < idx_dat2);
            Console.WriteLine();

            // *** ArrayList of KeyDat ***
            Console.WriteLine("*** ArrayList of KeyDat ***");
            Console.WriteLine();
            // create an ArrayList of KeyDat
            Console.WriteLine("Create an ArrayList of KeyDat ...");
            ArrayList<KeyDat<double, string>> list_key_dat = new ArrayList<KeyDat<double, string>>(new KeyDat<double, string>[] {
                new KeyDat<double, string>(2.4, "cats"),
                new KeyDat<double, string>(3.3, "dogs"),
                new KeyDat<double, string>(4.2, "lizards") });
            Console.WriteLine(list_key_dat);
            // sort descendingly
            Console.WriteLine("Sort descendingly ...");
            list_key_dat.Sort(new DescSort<KeyDat<double, string>>());
            Console.WriteLine(list_key_dat);
            // find item with bisection
            Console.WriteLine("Find item with bisection ...");
            int idx = list_key_dat.BinarySearch(new KeyDat<double, string>(3.3), new DescSort<KeyDat<double, string>>());
            Console.WriteLine(idx);
            idx = list_key_dat.BinarySearch(new KeyDat<double, string>(3), new DescSort<KeyDat<double, string>>());
            Console.WriteLine(~idx);
            // remove item
            Console.WriteLine("Remove item ...");
            list_key_dat.Remove(new KeyDat<double, string>(3.3));
            Console.WriteLine(list_key_dat);
            // get first and last item
            Console.WriteLine("Get first and last item ...");
            Console.WriteLine(list_key_dat.First);
            Console.WriteLine(list_key_dat.Last);
        }
    }
}
