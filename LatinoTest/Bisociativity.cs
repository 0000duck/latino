using System;
using System.Collections.Generic;
using System.Text;
using Latino.Model;
using Latino;

namespace BDocVisualizer
{
    public class Bisociativity : ISimilarity<SparseVector<double>.ReadOnly>
    {
        double m_k;

        public Bisociativity(double k)
        {
            m_k = k;
        }

        // *** ISimilarity<SparseVector<double>.ReadOnly> interface implementation ***

        public double GetSimilarity(SparseVector<double>.ReadOnly a, SparseVector<double>.ReadOnly b)
        {
            Utils.ThrowException(a == null ? new ArgumentNullException("a") : null);
            Utils.ThrowException(b == null ? new ArgumentNullException("b") : null);
            double bisoc = 0;
            int i = 0, j = 0;
            int a_count = a.Count;
            int b_count = b.Count;
            int count = 0;
            if (a_count == 0 || b_count == 0) { return 0; }
            ArrayList<int> a_idx = a.Inner.InnerIdx;
            ArrayList<double> a_dat = a.Inner.InnerDat;
            ArrayList<int> b_idx = b.Inner.InnerIdx;
            ArrayList<double> b_dat = b.Inner.InnerDat;
            int a_idx_i = a_idx[0];
            int b_idx_j = b_idx[0];
            while (true)
            {
                if (a_idx_i < b_idx_j)
                {
                    if (++i == a_count) { break; }
                    a_idx_i = a_idx[i];
                }
                else if (a_idx_i > b_idx_j)
                {
                    if (++j == b_count) { break; }
                    b_idx_j = b_idx[j];
                }
                else
                {
                    double tf_a = a_dat[i];
                    double tf_b = b_dat[j];
                    bisoc += Math.Pow(tf_a*tf_b,1.0/m_k)*(1.0-(Math.Abs(Math.Atan(tf_a)-Math.Atan(tf_b))/Math.Atan(1)));
                    count++;
                    if (++i == a_count || ++j == b_count) { break; }
                    a_idx_i = a_idx[i];
                    b_idx_j = b_idx[j];
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
