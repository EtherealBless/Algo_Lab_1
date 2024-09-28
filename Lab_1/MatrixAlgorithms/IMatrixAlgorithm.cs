using Lab_1.ArrayAlgorithms;
using Lab_1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.MatrixAlgorithms
{

    internal interface IMatrixAlgorithm<T> : IAlgorithm<Pair<IMatrix<T>, IMatrix<T>>>
    {
    }
}
