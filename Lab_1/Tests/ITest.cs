using Lab_1.ArrayAlgorithms;

namespace Lab_1.Tests
{
    public interface ITest<AlgorithmType, InputType> where AlgorithmType : IAlgorithm<InputType>
    {
        public Task<double> RunTest(AlgorithmType algorithm, InputType input);
    }
}