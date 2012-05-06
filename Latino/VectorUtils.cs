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

        /// <summary>
        /// Adds each element of vec to corresponding element of addTo vector
        /// addTo vector is being changed in place
        /// </summary>
        public static void AddInPlace(SparseVector<double> addTo, SparseVector<double> vec)
        {
            foreach (var vecIdx in vec.InnerIdx)
            {
                var num = addTo.TryGet(vecIdx, 0.0);
                addTo[vecIdx] = num + vec[vecIdx];
            }
        }

        /// <summary>
        /// Divides each element of dividee with corresponding element of divisor in place
        /// Dividee is being changed
        /// </summary>
        public static void DivideInPlace(SparseVector<double> dividee, SparseVector<double> divisor)
        {
            foreach (var divIdx in divisor.InnerIdx)
            {
                var num = dividee.TryGet(divIdx, 0.0);
                if (num < Double.Epsilon)
                    continue;

                dividee[divIdx] = num/divisor[divIdx];
            }
        }

        public static void Sqrt(SparseVector<double> vec)
        {
            var innerDat = vec.InnerDat;
            for (int i = 0; i < innerDat.Count; i++ )
                vec.SetDirect(i, Math.Sqrt(innerDat[i]));
        }
    }
}
