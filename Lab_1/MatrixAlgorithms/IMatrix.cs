using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.MatrixAlgorithms
{
    public interface IMatrix<T>
    {
        public T this[int i, int j] { get; set; }
        public T[][] this[Range i, Range j] { get; }
        public int Rows { get; }
        public int Cols { get; }

    }
}
