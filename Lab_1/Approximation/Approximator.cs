using CenterSpace.NMath.Core;
using Lab_1.ArrayAlgorithms;
using Lab_1.MatrixAlgorithms;
using Lab_1.PowAlgorithms;
using Lab_1.Utils;
using OpenTK.Graphics.OpenGL;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Approximation
{
    internal class Approximator
    {

        public static Dictionary<Type, NParameterizedFunction> AlgorithmsDifficulty = new()
        {
            {typeof(BubbleSort<int>), new ThreeParamSquare() },
            {typeof(QuickSort<int>), new FourParamNLogN() },
            {typeof(TimSort<int>), new FourParamNLogN() },
            {typeof(ConstFunction<int>), new OneParamConst() },
            {typeof(MultArray<int>), new TwoParamLinear() },
            {typeof(SumArray<int>), new TwoParamLinear() },
            {typeof(Multiplication<int>), new ThreeParamSquare() },
            {typeof(Pow<int>), new TwoParamLinear() },
            {typeof(RecPow<int>), new FourParamLogN() },
            {typeof(QuickPow<int>), new FourParamLogN() },
            {typeof(Polynomial<int>), new TwoParamLinear()}
        };

        public abstract class NParameterizedFunction : DoubleParameterizedFunction
        {
            public abstract int ParametersCount { get; }
        }

        public class ThreeParamSquare : NParameterizedFunction
        {
            public override int ParametersCount => 3;
            public override double Evaluate(DoubleVector parameters, double x)
            {
                if (parameters.Length != 3)
                {
                    throw new ArgumentException("Wrong number of parameters");
                }
                return parameters[0] * x * x + parameters[1] * x + parameters[2];
            }

            public override void GradientWithRespectToParams(DoubleVector parameters, double x, ref DoubleVector grad)
            {
                grad[0] = x * x;
                grad[1] = x;
                grad[2] = 1;
            }
        }
        public class FourParamNLogN : NParameterizedFunction
        {
            public override int ParametersCount => 1;

            public override double Evaluate(DoubleVector parameters, double x)
            {
                if (parameters.Length != ParametersCount)
                {
                    throw new ArgumentException("Wrong number of parameters");
                }
                Trace.WriteLine($"Evaluate: {NMathFunctions.Log(parameters[0] * x)}");

                return parameters[0] * NMathFunctions.Log(x);
            }

            public override void GradientWithRespectToParams(DoubleVector parameters, double x, ref DoubleVector grad)
            {
                grad[0] = NMathFunctions.Log(x);
                Trace.WriteLine($"Gradient: {grad}");
            }
        }

        public class OneParamConst : NParameterizedFunction
        {
            public override int ParametersCount => 1;

            public override double Evaluate(DoubleVector parameters, double x)
            {
                if (parameters.Length != 1)
                {
                    throw new ArgumentException("Wrong number of parameters");
                }

                return parameters[0];
            }

            public override void GradientWithRespectToParams(DoubleVector parameters, double x, ref DoubleVector grad)
            {
                grad[0] = 1;
            }
        }

        public class TwoParamLinear : NParameterizedFunction
        {
            public override int ParametersCount => 2;
            public override double Evaluate(DoubleVector parameters, double x)
            {
                if (parameters.Length != 2)
                {
                    throw new ArgumentException("Wrong number of parameters");
                }
                return parameters[0] * x + parameters[1];
            }

            public override void GradientWithRespectToParams(DoubleVector parameters, double x, ref DoubleVector grad)
            {
                grad[0] = x;
                grad[1] = 1;

            }
        }

        public class FourParamLogN : NParameterizedFunction
        {
            public override int ParametersCount => 4;
            public override double Evaluate(DoubleVector parameters, double x)
            {
                if (parameters.Length != 4)
                {
                    throw new ArgumentException("Wrong number of parameters");
                }

                return parameters[0] * Math.Log(x * parameters[2], parameters[1]) + parameters[3];
            }

            public override void GradientWithRespectToParams(DoubleVector parameters, double x, ref DoubleVector grad)
            {
                grad[0] = Math.Log(x * parameters[2]) / Math.Log(parameters[1]);
                grad[1] = -parameters[0] * Math.Log(x * parameters[2]) / parameters[1] / Math.Pow(Math.Log(x * parameters[1]), 2);
                grad[2] = parameters[0] / parameters[2] / Math.Log(parameters[1]);
                grad[3] = 1;
            }
        }

        private static readonly ThreeParamSquare _square = new ThreeParamSquare();

        public static DoubleVector Approximate<T>(IAlgorithm<T> algorithm, List<Point> points)
        {
            var function = AlgorithmsDifficulty[algorithm.GetType()] ?? throw new ArgumentException("Wrong algorithm type");

            DoubleVector parameters = new(3);

            var fitter = new BoundedOneVariableFunctionFitter<TrustRegionMinimizer>(function);

            DoubleVector start = new(string.Join(' ', Enumerable.Repeat("1", function.ParametersCount).ToArray()));

            var (x, y) = points.ToLists();

            DoubleVector solution = fitter.Fit(new DoubleVector(x.ToArray()), new DoubleVector(y.ToArray()), start);

            return solution;
        }

        public static DoubleVector ApproximateCoord<T, K>(IAlgorithm<T, K> algorithm, List<Coordinates> points)
        {
            Trace.WriteLine($"Approximation started: {algorithm.GetType()} with {points.Count} points");

            var function = AlgorithmsDifficulty[algorithm.GetType()] ?? throw new ArgumentException("Wrong algorithm type");

            var fitter = new OneVariableFunctionFitter<TrustRegionMinimizer>(function);

            DoubleVector start = new(string.Join(' ', Enumerable.Repeat("1,1", function.ParametersCount).ToArray()));

            var (x, y) = points.ToLists();

            DoubleVector solution = fitter.Fit(new DoubleVector(x.ToArray()), new DoubleVector(y.ToArray()), start);

            Trace.WriteLine($"Approximation: {solution}");

            return solution;
        }

    }
}
