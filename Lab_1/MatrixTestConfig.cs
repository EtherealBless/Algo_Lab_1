using Lab_1.Generators;

namespace Lab_1
{
    public class MatrixTestConfig
    {
        public int MatrixSize { get; set; } = 1000;

        public int Iterations { get; set; } = 100;

        public IMatrixGenerator<int> MatrixGenerator { get; set; } = new IntMatrixGenerator();
    }
}