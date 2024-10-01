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
            // arrays
            {typeof(BubbleSort<int>), new ThreeParamSquare() },
            {typeof(QuickSort<int>), new FiveParamNLogN() },
            {typeof(TimSort<int>), new FiveParamNLogN() },
            {typeof(ConstFunction<int>), new OneParamConst() },
            {typeof(MultArray<int>), new TwoParamLinear() },
            {typeof(SumArray<int>), new TwoParamLinear() },
            {typeof(Polynomial<int>), new TwoParamLinear()},
            {typeof(BlockSort<int>), new FiveParamNLogN() },
            {typeof(StrandSort<int>), new ThreeParamSquare() },
            {typeof(BogoSort<int>), new FourParamCube() },
            // matrix
            {typeof(Multiplication<int>), new FourParamCube() },
            // pow
            {typeof(Pow<int>), new TwoParamLinear() },
            {typeof(RecPow<int>), new FiveParamLogN() },
            {typeof(QuickPow<int>), new FiveParamLogN() },
            {typeof(ClassicPow<int>), new FiveParamLogN() }
        };

        public abstract class NParameterizedFunction : DoubleParameterizedFunction
        {
            public abstract int ParametersCount { get; }
            public abstract string Formula { get; } // = "y = {0:N}*x^2 + {1:N}*x + {2:N}";
        }

        public class ThreeParamSquare : NParameterizedFunction
        {
            public override int ParametersCount => 3;
            public override string Formula { get; } = "y = {0:N}*x^2 + {1:N}*x + {2:N}";
            public override double Evaluate(DoubleVector parameters, double x)
            {
                if (parameters.Length != ParametersCount)
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

        public class FourParamCube : NParameterizedFunction
        {
            public override int ParametersCount => 4;
            public override string Formula { get; } = "y = {0:N}*x^3 + {1:N}*x^2 + {2:N}*x + {3:N}";
            public override double Evaluate(DoubleVector parameters, double x)
            {
                if (parameters.Length != ParametersCount)
                {
                    throw new ArgumentException("Wrong number of parameters");
                }
                return parameters[0] * x * x * x + parameters[1] * x * x + parameters[2] * x + parameters[3];
            }

            public override void GradientWithRespectToParams(DoubleVector parameters, double x, ref DoubleVector grad)
            {
                grad[0] = x * x * x;
                grad[1] = x * x;
                grad[2] = x;
                grad[3] = 1;
            }
        }
        public class FiveParamNLogN : NParameterizedFunction
        {
            public override int ParametersCount => 5;
            public override string Formula { get; } = "y = {0:N}*x * log({1:N}*x + {2:N}, {3:N}) + {4:N}";
            public override double Evaluate(DoubleVector parameters, double x)
            {
                if (parameters.Length != ParametersCount)
                {
                    throw new ArgumentException("Wrong number of parameters");
                }
                if (x * parameters[2] < 0 || parameters[1] < 0 || parameters[1] == 1)
                {
                    //throw new ArgumentException("Wrong parameters");
                    return double.NaN;
                }
                // a * log(cx+d, b) + f
                return parameters[0] * x * Math.Log(parameters[1] * x + parameters[2], parameters[3]) + parameters[4];
            }

            // 0 - a
            // 1 - b
            // 2 - c
            // 3 - d
            // 4 - f
            public override void GradientWithRespectToParams(DoubleVector parameters, double x, ref DoubleVector grad)
            {
                //Log[d + c x]/Log[b]
                grad[0] = x * Math.Log(parameters[2] * x + parameters[3]) / Math.Log(parameters[1]);

                //-((a Log[d + c x])/(b Log[b]^2))
                grad[1] = (-parameters[0] * x * Math.Log(parameters[2] * x + parameters[3])) / (Math.Log(parameters[1]) * Math.Log(parameters[1])) / parameters[1];

                //(a x)/((d + c x) Log[b])
                grad[2] = parameters[0] * x * x / ((parameters[3] + parameters[2] * x) * Math.Log(parameters[1]));

                //a/((d + c x) Log[b])
                grad[3] = parameters[0] * x / ((parameters[3] + parameters[2] * x) * Math.Log(parameters[1]));

                grad[4] = 1;
            }
        }

        public class OneParamConst : NParameterizedFunction
        {
            public override int ParametersCount => 1;
            public override string Formula { get; } = "y = {0:N}";
            public override double Evaluate(DoubleVector parameters, double x)
            {
                if (parameters.Length != ParametersCount)
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
            public override string Formula { get; } = "y = {0:N}*x + {1:N}";
            public override double Evaluate(DoubleVector parameters, double x)
            {
                if (parameters.Length != ParametersCount)
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

        public class FiveParamLogN : NParameterizedFunction
        {
            public override int ParametersCount => 5;
            public override string Formula { get; } = "y = {0:N}*Log({1:N}*x + {2:N}, {3:N}) + {4:N}";
            public override double Evaluate(DoubleVector parameters, double x)
            {
                if (parameters.Length != ParametersCount)
                {
                    throw new ArgumentException("Wrong number of parameters");
                }
                if (x * parameters[2] < 0 || parameters[1] < 0 || parameters[1] == 1)
                {
                    return double.NaN;
                }

                return parameters[0] * Math.Log(x * parameters[2] + parameters[3], parameters[1]) + parameters[4];
            }

            public override void GradientWithRespectToParams(DoubleVector parameters, double x, ref DoubleVector grad)
            {
                //Log[d + c x]/Log[b]
                grad[0] = Math.Log(x * parameters[2] + parameters[3]) / Math.Log(parameters[1]);
                //-((a Log[d + c x])/(b Log[b]^2))
                grad[1] = (-parameters[0] * Math.Log(x * parameters[2] + parameters[3])) / (Math.Log(parameters[1]) * Math.Log(parameters[1])) / parameters[1];

                //(a x)/((d + c x) Log[b])
                grad[2] = parameters[0] * x / ((parameters[3] + x * parameters[2]) * Math.Log(parameters[1]));

                //a/((d + c x) Log[b])
                grad[3] = parameters[0] / ((parameters[3] + x * parameters[2]) * Math.Log(parameters[1]));

                grad[4] = 1;
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

        public static DoubleVector ApproximateCoord<T, K>(IAlgorithm<T, K> algorithm, List<Coordinates> points, DoubleVector? start = null)
        {
            Trace.WriteLine($"Approximation started: {algorithm.GetType()} with {points.Count} points");
            
            var function = AlgorithmsDifficulty[algorithm.GetType()] ?? throw new ArgumentException("Wrong algorithm type");

            var fitter = new OneVariableFunctionFitter<TrustRegionMinimizer>(function);

            start = (start != null && start.Length == function.ParametersCount) ? 
                    start : 
                    new(string.Join(' ', Enumerable.Repeat("0.5", function.ParametersCount).ToArray()));

            var (x, y) = points.ToLists();

            //if (x[0] == 0)
            //{
            //    x = x.Skip(1).ToList();
            //    y = y.Skip(1).ToList();
            //}

            DoubleVector solution = fitter.Fit(new DoubleVector(x.ToArray()), new DoubleVector(y.ToArray()), start);

            Trace.WriteLine($"Approximation: {solution}");

            return solution;
        }
    }
}
