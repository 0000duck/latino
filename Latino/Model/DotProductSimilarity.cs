/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    DotProductSimilarity.cs
 *  Desc:    Similarity implementation
 *  Created: Dec-2008
 *
 *  Authors: Miha Grcar, Matjaz Jursic
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class DotProductSimilarity
       |
       '-----------------------------------------------------------------------
    */
    public class DotProductSimilarity : ISimilarity<SparseVector<double>.ReadOnly>, ISimilarity<SparseVector<double>>
    {
        public static DotProductSimilarity mInstance
            = new DotProductSimilarity();

        public DotProductSimilarity()
        {
        }

        public DotProductSimilarity(BinarySerializer reader)
        {
        }

        public static DotProductSimilarity Instance
        {
            get { return mInstance; }
        }

        // *** ISimilarity<SparseVector<double>> interface implementation ***

        public double GetSimilarity(SparseVector<double> a, SparseVector<double> b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            double dotProd = 0;
            int i = 0, j = 0;
            int aCount = a.Count;
            int bCount = b.Count;
            if (aCount == 0 || bCount == 0) { return 0; }
            ArrayList<int> aIdx = a.InnerIdx;
            ArrayList<double> aDat = a.InnerDat;
            ArrayList<int> bIdx = b.InnerIdx;
            ArrayList<double> bDat = b.InnerDat;
            int aIdx_i = aIdx[0];
            int bIdx_j = bIdx[0];
            while (true)
            {
                if (aIdx_i < bIdx_j)
                {
                    if (++i == aCount) { break; }
                    aIdx_i = aIdx[i];
                }
                else if (aIdx_i > bIdx_j)
                {
                    if (++j == bCount) { break; }
                    bIdx_j = bIdx[j];
                }
                else
                {
                    dotProd += aDat[i] * bDat[j];
                    if (++i == aCount || ++j == bCount) { break; }
                    aIdx_i = aIdx[i];
                    bIdx_j = bIdx[j];
                }
            }
            return dotProd;
        }

        // *** ISimilarity<SparseVector<double>.ReadOnly> interface implementation ***
        public double GetSimilarity(SparseVector<double>.ReadOnly a, SparseVector<double>.ReadOnly b)
        {
            return GetSimilarity(a.Inner, b.Inner);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
        }
    }
}