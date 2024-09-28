using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.MatrixAlgorithms
{
    internal class Matrix<T> : IMatrix<T>
    {
        public T[][] array { get; set; }

        public int Rows => array.GetLength(0);

        public int Cols => array.GetLength(1);

        public Matrix(T[,] array)
        {
            this.array = new T[array.GetLength(0)][];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                this.array[i] = new T[array.GetLength(1)];
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    this.array[i][j] = array[i, j];
                }
            }

        }

        public Matrix(T[][] array)
        {
            this.array = new T[array.GetLength(0)][];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                this.array[i] = new T[array.GetLength(1)];
                Array.Copy(array[i], this.array[i], array.GetLength(1));
            }
        }

        public T this[int i, int j]
        {
            get { return array[i][j]; }
            set { array[i][j] = value; }
        }

        public T[][] this[Range i, Range j]
        {
            get { return array[i][j]; }
        }
    }
}
