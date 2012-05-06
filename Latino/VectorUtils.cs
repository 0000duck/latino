using System;

namespace Latino
{
    public static class VectorUtils
    {
        public static SparseVector<double> Multiply(double scalar, SparseVector<double> vector)
        {
            var newVector = vector.DeepClone();
            for (int i = 0; i < newVector.InnerDat.Count; i++ )
            {
                newVector.SetDirect(i, newVector.InnerDat[i] * scalar);
            }
            return newVector;
        }

        public static SparseVector<double> Add(SparseVector<double> v1, SparseVector<double> v2)
        {
            var newVector = new SparseVector<double>();
            var v1LastI = v1.Count > 0 ? v1.Last.Idx : 0;
            var v2LastI = v2.Count > 0 ? v2.Last.Idx : 0;
            var limit = Math.Max(v1LastI, v2LastI);

            for (int i = 0; i < limit; i++)
            {
                var sum = v1.TryGet(i, 0.0) + v2.TryGet(i, 0.0);
                if (sum > Double.Epsilon)
                    newVector[i] = sum;
            }

            return newVector;
        }

        public static void Sqrt(SparseVector<double> vec)
        {
            var innerDat = vec.InnerDat;
            for (int i = 0; i < innerDat.Count; i++ )
                vec.SetDirect(i, Math.Sqrt(innerDat[i]));
        }
    }
}
