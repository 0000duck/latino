using System;
using System.Collections.Generic;
using System.Text;
using Latino.Model;
using Latino;

namespace BDocVisualizer
{
    public class Bisociativity : ISimilarity<SparseVector<double>.ReadOnly>
    {
        double mK;

        public Bisociativity(double k)
        {
            mK = k;
        }

        // *** ISimilarity<SparseVector<double>.ReadOnly> interface implementation ***

        public double GetSimilarity(SparseVector<double>.ReadOnly a, SparseVector<double>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            double bisoc = 0;
            int i = 0, j = 0;
            int aCount = a.Count;
            int bCount = b.Count;
            int count = 0;
            if (aCount == 0 || bCount == 0) { return 0; }
            ArrayList<int> aIdx = a.Inner.InnerIdx;
            ArrayList<double> aDat = a.Inner.InnerDat;
            ArrayList<int> bIdx = b.Inner.InnerIdx;
            ArrayList<double> bDat = b.Inner.InnerDat;
            int aIdxI = aIdx[0];
            int bIdxJ = bIdx[0];
            while (true)
            {
                if (aIdxI < bIdxJ)
                {
                    if (++i == aCount) { break; }
                    aIdxI = aIdx[i];
                }
                else if (aIdxI > bIdxJ)
                {
                    if (++j == bCount) { break; }
                    bIdxJ = bIdx[j];
                }
                else
                {
                    double tfA = aDat[i];
                    double tfB = bDat[j];
                    bisoc += Math.Pow(tfA*tfB,1.0/mK)*(1.0-(Math.Abs(Math.Atan(tfA)-Math.Atan(tfB))/Math.Atan(1)));
                    count++;
                    if (++i == aCount || ++j == bCount) { break; }
                    aIdxI = aIdx[i];
                    bIdxJ = bIdx[j];
                }
            }
            return bisoc /*/ (double)count*/;
        }

        public void Save(BinarySerializer writer)
        {
            throw new NotImplementedException();
        }
    }
}
