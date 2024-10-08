﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.MatrixAlgorithms
{
    internal class Matrix<T> : IMatrix<T>
    {
        public T[][] array { get; set; } = [];

        public int Rows => array == null ? 0 : array.GetLength(0);

        public int Cols => Rows == 0 ? 0 : array[0].GetLength(0);

        public Matrix()
        {
            //array = new T[0][];
        }

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
                this.array[i] = new T[array[0].GetLength(0)];
                Array.Copy(array[i], this.array[i], array[0].GetLength(0));
            }
        }

        public T this[int i, int j]
        {
            get { return array[i][j]; }
            set { array[i][j] = value; }
        }

        public T[][] this[Range i, Range j]
        {
            get
            {
                int iLength = i.End.Value - i.Start.Value;
                int jLength = j.End.Value - j.Start.Value;
                T[][] result = new T[iLength][];

                for (int k = 0; k < iLength; k++)
                {
                    result[k] = new T[jLength];

                    Array.Copy(array[i.Start.Value + k], result[k], jLength);
                }

                return result;
            }
        }
    }
}
