using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Lab_1.Utils;

namespace Lab_1.MatrixAlgorithms
{
    internal class Multiplication<T> : IMatrixAlgorithm<T> where T : IAdditionOperators<T, T, T>, IMultiplyOperators<T, T, T>, IAdditiveIdentity<T, T>
    {
        public Task Execute(Pair<IMatrix<T>, IMatrix<T>> pair)
        {
            IMatrix<T> a = pair.First!;
            IMatrix<T> b = pair.Second!;
            IMatrix<T> result = new Matrix<T>(new T[a.Rows, b.Cols]);

            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < b.Cols; j++)
                {
                    T sum = T.AdditiveIdentity;

                    for (int k = 0; k < a.Cols; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }
                }
            }
            return Task.CompletedTask;
        }

    }
}
