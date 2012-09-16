/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Program.cs
 *  Desc:    LATINO Core tutorials
 *  Created: Oct-2011
 *
 *  Author:  Miha Grcar
 * 
 *  This library is free software: you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation, either
 *  version 3 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library.  If not, see <http://www.gnu.org/licenses/>.
 *
 ***************************************************************************/

#define EXAMPLE9 // change this to select a different example

using System;
using System.IO;
using Latino;

namespace Latino.Tutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Example 1: Sorting an array of key-value pairs and creating its read-only wrapper
#if EXAMPLE1
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
#endif
            #endregion

            #region Example 2: Cloning an array of referenced integers shallowly and deeply
#if EXAMPLE2
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
#endif
            #endregion

            #region Example 3: Creating two sets of integers and comparing them by reference and by content
#if EXAMPLE3
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
#endif
            #endregion

            #region Example 4: Serializing and deserializing LATINO objects
#if EXAMPLE4
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
#endif
            #endregion

            #region Example 5: Performing basic operations on sets
#if EXAMPLE5
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
#endif
            #endregion

            #region Example 6: Performing basic operations on sparse vectors
#if EXAMPLE6
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
#endif
            #endregion

            #region Example 7: Performing basic operations on sparse matrices
#if EXAMPLE7
            // create a SparseMatrix<double>
            SparseMatrix<double> mat = new SparseMatrix<double>();
            mat[0] = new SparseVector<double>(new IdxDat<double>[] { 
                new IdxDat<double>(0, 1.1),
                new IdxDat<double>(1, 1.2)});
            mat[1] = new SparseVector<double>(new IdxDat<double>[] { 
                new IdxDat<double>(1, 2.2)});
            mat[2] = new SparseVector<double>(new IdxDat<double>[] { 
                new IdxDat<double>(1, 3.2),
                new IdxDat<double>(2, 3.3)});
            // output it to the console 
            Console.WriteLine(mat.ToString("E"));
            Console.WriteLine();
            // traverse the matrix
            foreach (IdxDat<SparseVector<double>> row in mat)
            {
                foreach (IdxDat<double> item in row.Dat)
                {
                    Console.Write("({0},{1},{2}) ", row.Idx, item.Idx, item.Dat);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            // change a value
            mat[1, 1] = -2.2;
            Console.WriteLine(mat.ToString("E"));
            Console.WriteLine();
            // remove an element
            mat.RemoveAt(1, 1);
            Console.WriteLine(mat.ToString("E"));
            Console.WriteLine();
            // get the number of dimensions
            Console.WriteLine("rows: {0}, cols: {1}", mat.GetLastNonEmptyRowIdx() + 1, mat.GetLastNonEmptyColIdx() + 1);
            Console.WriteLine();
            // transpose the matrix
            SparseMatrix<double> matTr = mat.GetTransposedCopy();
            Console.WriteLine(matTr.ToString("E"));
#endif
            #endregion

            #region Example 8: Accessing sparse vector elements directly
#if EXAMPLE8
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
#endif
            #endregion

            #region Example 9: Using the logging tool
#if EXAMPLE9
            // create two loggers
            Logger logger1 = Logger.GetLogger("Latino.Tutorials.Example9.Logger1");
            Logger logger2 = Logger.GetLogger("Latino.Tutorials.Example9.Logger2");
            // output a message and a warning
            logger1.Info("Main", "This message is brought to you by Logger 1.");
            logger2.Warn("Main", "This warning is brought to you by Logger 2.");
            // change the output format of Logger 2
            logger2.CustomOutput = new Logger.CustomOutputDelegate(delegate(string loggerName, Logger.Level level, string funcName, Exception exception, string message, object[] msgArgs) {
                Console.WriteLine("{0} says: \"{1}\"", loggerName, string.Format(message, msgArgs));
            });
            logger2.LocalOutputType = Logger.OutputType.Custom;
            // output the message and warning again
            logger1.Info("Main", "This message is brought to you by Logger 1.");
            logger2.Warn("Main", "This warning is brought to you by Logger 2.");
            // set both loggers to output only warnings, errors, and fatal errors
            Logger.GetRootLogger().LocalLevel = Logger.Level.Warn;
            logger1.Trace("Main", "This trace message is brought to you by Logger 1."); // this will not be displayed
            logger1.Debug("Main", "This debug message is brought to you by Logger 1."); // this will not be displayed
            logger2.Info("Main", "This message is brought to you by Logger 2."); // this will not be displayed
            logger2.Warn("Main", "This warning is brought to you by Logger 2.{0}");
#endif
            #endregion
        }
    }
}
