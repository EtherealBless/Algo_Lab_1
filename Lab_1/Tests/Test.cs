
using Lab_1.ArrayAlgorithms;
using System.Diagnostics;
using static OpenTK.Graphics.OpenGL.GL;

namespace Lab_1.Tests
{
    internal class Test<AlgorithmType, InputType> : ITest<AlgorithmType, InputType> where AlgorithmType : IAlgorithm<InputType>
    {
        public Task<double> RunTest(AlgorithmType algorithm, InputType input)
        {
            return Task.Run(() =>
            {
                return TestOnce(algorithm, input); 
            });
        }

        private static double TestOnce(AlgorithmType algorithm, InputType input)
        {
            var watch = new Stopwatch();
            watch.Start();
            algorithm.Execute(input).Wait();
            watch.Stop();
            return watch.ElapsedTicks;
        }

    }
}