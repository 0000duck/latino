/*=====================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Tutorial2.cs
 *  Version:       1.0
 *  Desc:          Tutorial 2: SparseVector and SparseMatrix
 *  Author:        Miha Grcar
 *  Created on:    Dec-2009
 *  Last modified: Dec-2009
 *  Revision:      Dec-2009
 *
 **********************************************************************/

using System;
using Latino;
using System.Collections.Generic;

namespace LatinoTutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // *** SparseVector ***
            Console.WriteLine("*** SparseVector ***");
            Console.WriteLine();
            // create a SparseVector
            Console.WriteLine("Create a SparseVector ...");
            SparseVector<string> vec = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(1, "a"),
                new IdxDat<string>(3, "b"),
                new IdxDat<string>(4, "c"),
                new IdxDat<string>(6, "d"),
            });
            Console.WriteLine(vec);
            // add some more items
            Console.WriteLine("Add some more items ...");
            // ... at the end
            vec.Add("E"); 
            vec.AddRange(new string[] { "F", "G" }); 
            // ... at specific places 
            vec.AddRange(new IdxDat<string>[] {
                new IdxDat<string>(2, "AB"),
                new IdxDat<string>(10, "H"),
            });
            vec[11] = "i";
            Console.WriteLine(vec);
            // get items
            Console.WriteLine("Get items ...");
            Console.WriteLine(vec[1]);
            Console.WriteLine(vec.TryGet(2, "missing"));
            Console.WriteLine(vec.TryGet(5, "missing"));
            // set items
            Console.WriteLine("Set items ...");
            vec[2] = "ab";
            vec[10] = "h";
            Console.WriteLine(vec);
            // check for items
            Console.WriteLine("Check for items ...");
            Console.WriteLine(vec.ContainsAt(2));
            Console.WriteLine(vec.ContainsAt(5));
            // create another SparseVector
            Console.WriteLine("Create another SparseVector ...");
            SparseVector<string> vec2 = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(0, "!"),
                new IdxDat<string>(2, "@"),
                new IdxDat<string>(3, "#"),
                new IdxDat<string>(5, "$"),
            });
            Console.WriteLine(vec2);
            // get first and last non-empty index
            Console.WriteLine("Get first and last non-empty index ...");
            Console.WriteLine(vec.FirstNonEmptyIndex);
            Console.WriteLine(vec.LastNonEmptyIndex);
            // get first and last item
            Console.WriteLine("Get first and last item ...");
            Console.WriteLine(vec.First);
            Console.WriteLine(vec.Last);
            // concatenate 
            Console.WriteLine("Concatenate ...");
            vec.Append(vec2, vec.LastNonEmptyIndex + 1);
            Console.WriteLine(vec);
            vec2.Append(vec, vec2.LastNonEmptyIndex + 1);
            Console.WriteLine(vec2);
            // get length
            Console.WriteLine("Get length ...");
            Console.WriteLine(vec.Count);
            // remove item
            Console.WriteLine("Remove item ...");
            vec.RemoveAt(2);
            Console.WriteLine(vec);
            // directly access to items
            Console.WriteLine("Directly access to items ...");
            int idx = vec.GetDirectIdx(3);
            Console.WriteLine(idx);
            vec.SetDirect(idx, "bbb");
            Console.WriteLine(vec);
            Console.WriteLine(vec.GetIdxDirect(idx));
            Console.WriteLine(vec.GetDatDirect(idx));
            Console.WriteLine(vec.GetDirect(idx));
            vec.RemoveDirect(idx);
            Console.WriteLine(vec);
            // perform unary operation
            Console.WriteLine("Perform unary operation ...");
            vec.PerformUnaryOperation(delegate(string item) { return item.ToUpper(); });
            Console.WriteLine(vec);
            // merge
            Console.WriteLine("Merge ...");
            vec.Merge(vec2, delegate(string a, string b) { return string.Format("{0}+{1}", a, b); });
            Console.WriteLine(vec);
            // purge
            Console.WriteLine("Purge items ...");
            vec.PurgeAt(1);
            Console.WriteLine(vec);
            vec.PurgeAt(1);
            Console.WriteLine(vec);
            Console.WriteLine();

            // *** SparseMatrix ***
            Console.WriteLine("*** SparseMatrix ***");
            Console.WriteLine();
            // create a SparseMatrix
            Console.WriteLine("Create a SparseMatrix ...");
            SparseMatrix<string> matrix = new SparseMatrix<string>();
            matrix[0] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(1, "a"),
                new IdxDat<string>(3, "b"),
                new IdxDat<string>(4, "c")
            });
            matrix[2] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(2, "d"),
                new IdxDat<string>(4, "e"),
                new IdxDat<string>(5, "f")
            });
            matrix[3] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(0, "g"),
                new IdxDat<string>(3, "h"),
                new IdxDat<string>(5, "i")
            });
            Console.WriteLine(matrix.ToString("E"));
            // get items
            Console.WriteLine("Get items ...");
            Console.WriteLine(matrix[0, 1]);
            Console.WriteLine(matrix[2, 2]);
            // set items
            Console.WriteLine("Set items ...");
            matrix[0, 1] = "j";
            matrix[2, 3] = "k";
            matrix[3, 4] = "l";
            Console.WriteLine(matrix.ToString("E"));
            // check for items
            Console.WriteLine("Check for items ...");
            Console.WriteLine(matrix.ContainsAt(0, 1));
            Console.WriteLine(matrix.ContainsAt(1, 1));
            // create another SparseMatrix
            Console.WriteLine("Create another SparseMatrix ...");
            SparseMatrix<string> matrix2 = new SparseMatrix<string>();
            //matrix2[0] = new SparseVector<string>(new IdxDat<string>[] {
            //    new IdxDat<string>(1, "a"),
            //    new IdxDat<string>(3, "b"),
            //    new IdxDat<string>(4, "c")
            //});
            //matrix2[2] = new SparseVector<string>(new IdxDat<string>[] {
            //    new IdxDat<string>(2, "d"),
            //    new IdxDat<string>(4, "e"),
            //    new IdxDat<string>(5, "f")
            //});
            //matrix2[3] = new SparseVector<string>(new IdxDat<string>[] {
            //    new IdxDat<string>(0, "g"),
            //    new IdxDat<string>(3, "h"),
            //    new IdxDat<string>(5, "i")
            //});
            Console.WriteLine(matrix2);


            SortedDictionary<double, int> dict = new SortedDictionary<double, int>();
            dict.Add(3.3, 3);
            dict.Add(2.2, 2);
            dict.Add(1.1, 1);
            IEnumerator<int> en = dict.Values.GetEnumerator();
            en.MoveNext();
            Console.WriteLine(en.Current);
            
        }
    }
}
