using Lab_1.MatrixAlgorithms;
using Lab_1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Generators
{
    internal interface IMatrixGenerator<NumberType>: IGenerator<Pair<IMatrix<NumberType>, IMatrix<NumberType>>>
    {
        Pair<IMatrix<NumberType>, IMatrix<NumberType>> GenerateMatrices(int rows, int columns);
    }
}
