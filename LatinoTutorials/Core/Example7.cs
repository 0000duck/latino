using System;
using Latino;

namespace Latino.Tutorials
{
    class Program
    {
        static void Main(string[] args)
        {
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
        }
    }
}