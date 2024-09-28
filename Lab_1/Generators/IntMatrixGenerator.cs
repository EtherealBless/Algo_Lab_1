using Lab_1.MatrixAlgorithms;
using Lab_1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Generators
{
    internal class IntMatrixGenerator : IMatrixGenerator<int>
    {
        private IMatrix<int> matrix;


        public IntMatrixGenerator(int expectedRows, int expectedColumns) {
            if (expectedColumns< 0 || expectedRows < 0)
            {
                throw new ArgumentException("expectedRows and expectedColumns must be positive");
            }

            matrix = GenerateMatrix(expectedRows, expectedColumns);

        }


        public IMatrix<int> GenerateMatrix(int size)
        {
            if (size > matrix.Rows || size > matrix.Cols)
            { 
                return GenerateMatrix(size, size);
            }
            return new Matrix<int>(matrix[new Range(0, size), new Range(0, size)]);
        }

        public IMatrix<int> GenerateMatrix(int rows, int columns)
        {
            if (rows > matrix.Rows || columns > matrix.Cols)
            {
                Random random = new(0);

                int[,] array = new int[rows, columns];

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        array[i, j] = random.Next();
                    }
                }

                matrix = new Matrix<int>(array);
            }

            return new Matrix<int>(matrix[new Range(0, rows), new Range(0, columns)]);
        }

        public Pair<IMatrix<int>, IMatrix<int>> GenerateMatrices(int rows, int columns)
        {
            return new Pair<IMatrix<int>, IMatrix<int>>(GenerateMatrix(rows, columns), GenerateMatrix(rows, columns));
        }


        public Pair<IMatrix<int>, IMatrix<int>> Generate(int seed)
        {
            throw new NotImplementedException();
        }
    }
}
