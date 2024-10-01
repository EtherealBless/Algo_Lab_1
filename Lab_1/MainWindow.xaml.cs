using CenterSpace.NMath.Core;
using Lab_1.Approximation;
using Lab_1.ArrayAlgorithms;
using Lab_1.Generators;
using Lab_1.MatrixAlgorithms;
using Lab_1.Tests;
using Lab_1.Utils;
using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using static Lab_1.Approximation.Approximator;
using Point = Lab_1.Utils.Point;

namespace Lab_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ArrayTestConfig ArrayTestConfig { get; set; } = new();
        public MatrixTestConfig MatrixTestConfig { get; set; } = new();

        public PowTestConfig PowTestConfig { get; set; } = new();

        private CancellationTokenSource? _cancelTokenRun;

        private bool TEST = false;

        public MainWindow()
        {
            InitializeComponent();

            if (TEST)
            {
                ArrayTestConfig.Iterations = 1;
                ArrayTestConfig.ArraySize = 100;
                ArrayTestConfig.SelectedAlgorithm = ArrayTestConfig.Algorithms["QuickSort"];

                StartArrayExperiments(null, null);
            }
        }

        private async Task RunPlotUpdate<T, K>(WpfPlot plot, int refreshRate, IAlgorithm<T, K> algorithm, List<Coordinates> source, CancellationToken token = default)
        {
            ClearApproximation();
            plot.Plot.Axes.SetLimitsX(0, 200);
            plot.Plot.Axes.SetLimitsY(0, 200);
            FunctionPlot? functionPlot = null;
            var prevSourceCount = source.Count;
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(refreshRate, CancellationToken.None);
                //var limits = plot.Plot.Axes.GetLimits();
                //plot.Plot.Axes.SetLimitsX(0, maxX);
                ////plot.Plot.Axes.SetLimitsY(0, limits.Top);
                Trace.WriteLine($"Drawing {algorithm}...");
                lock (source)
                {
                    functionPlot = DrawApproximation(algorithm, source, functionPlot);
                    functionPlot.MaxX = source.Count * 1.1;

                    plot.Plot.Axes.AutoScale();
                    plot.Refresh();
                    prevSourceCount = source.Count;
                }

                Trace.WriteLine($"Done");
            }
        }


        private async Task RunCommontExperements<T>(IAlgorithm<T> algorithm, int maxSize, int iterations, IGenerator<T> generator, CancellationToken token = default)
        {
            TestManager manager = new();
            List<Coordinates> source = [];
            MainPlot.Plot.Add.Scatter(new ScatterSourceCoordinatesList(source));
            FunctionPlot? functionPlot = null;

            CancellationTokenSource cancelTokenSourceUpdate = new();
            var cancelTokenUpdate = cancelTokenSourceUpdate.Token;

            _ = Task.Run(async () => await RunPlotUpdate(MainPlot, 100, algorithm, source, cancelTokenUpdate), cancelTokenUpdate);

            int n = 0;
            await foreach (var point in manager.TestAlgorithm(
                algorithm,
                generator,
                maxSize))
            {
                if (n % 10 == 0 && token.IsCancellationRequested)
                {
                    cancelTokenSourceUpdate.Cancel();
                    return;
                }

                source.Add(new(point.X, point.Y));
                n++;
            }

            for (int i = 1; i < iterations; i++)
            {
                n = 0;
                await foreach (var point in manager.TestAlgorithm(
                    algorithm,
                    generator,
                    maxSize))
                {
                    if (n % 10 == 0 && token.IsCancellationRequested)
                    {
                        cancelTokenSourceUpdate.Cancel();
                        return;
                    }
                    source[n] = new(point.X, (source[n].Y * i + point.Y) / (i + 1));

                    n++;

                }
            }
            cancelTokenSourceUpdate.Cancel();

        }

        private void ClearApproximation()
        {
            Trace.WriteLine($"[{nameof(ClearApproximation)}] Clearing approximation...");
            lock (MainPlot.Plot)
            {
                MainPlot.Plot.Remove<FunctionPlot>();
                //Trace.WriteLine($"{MainPlot.Plot.GetPlottables()}");
                //MainPlot.Refresh();
            }
        }

        private FunctionPlot DrawApproximation<T, K>(IAlgorithm<T, K> algorithm, List<Coordinates> source, FunctionPlot? functionPlot)
        {
            var res = ApproximateCoord(algorithm, source);
            var approximationFunction = AlgorithmsDifficulty[algorithm.GetType()] ?? throw new ArgumentException("Wrong algorithm type");
            Trace.WriteLine($"Approximation function: {approximationFunction}. Result: {res}");
            if (functionPlot != null)
            {
                MainPlot.Plot.Remove(functionPlot);
            }
            functionPlot = MainPlot.Plot.Add.Function((double x) => approximationFunction!.Evaluate(res, x));
            functionPlot.LineStyle.Width = 3;
            functionPlot.LineColor = Color.FromColor(System.Drawing.Color.OrangeRed);
            functionPlot.MinX = 0;
            functionPlot.MaxX = source.Count;
            return functionPlot;
        }

        private async Task RunCommontExperements<T, K>(IAlgorithm<T, Task<K>> algorithm, int maxSize, int iterations, IGenerator<T> generator, CancellationToken token = default)
        {
            TestManager manager = new();
            List<Coordinates> source = [];
            MainPlot.Plot.Add.Scatter(new ScatterSourceCoordinatesList(source));
            FunctionPlot? functionPlot = null;
            CancellationTokenSource cancelTokenSourceUpdate = new();
            var cancelTokenUpdate = cancelTokenSourceUpdate.Token;
            _ = Task.Run(async () => await RunPlotUpdate(MainPlot, 100, algorithm, source, cancelTokenUpdate), cancelTokenUpdate);
            int n = 0;
            await foreach (var point in manager.TestAlgorithm(
                algorithm,
                generator,
                maxSize))
            {
                if (n % 10 == 0 && token.IsCancellationRequested)
                {
                    cancelTokenSourceUpdate.Cancel();
                    return;
                }
                source.Add(new(point.X, point.Y));

                if (n % 100 == 0)
                {
                    //functionPlot = DrawApproximation(source, functionPlot);
                }
                n++;
            }

            for (int i = 1; i < iterations; i++)
            {
                n = 0;
                await foreach (var point in manager.TestAlgorithm(
                    algorithm,
                    generator,
                    maxSize))
                {
                    if (n % 10 == 0 && token.IsCancellationRequested)
                    {
                        cancelTokenSourceUpdate.Cancel();
                        return;
                    }
                    source[n] = new(point.X, (source[n].Y * i + point.Y) / (i + 1));
                    n++;

                }
            }
            cancelTokenSourceUpdate.Cancel();

        }

        private CancellationTokenSource ResetCancelToken()
        {
            if (_cancelTokenRun != null)
            {
                _cancelTokenRun.Cancel();
                _cancelTokenRun.Dispose();
                _cancelTokenRun = null;
                MainPlot.Plot.Clear();
            }
            _cancelTokenRun = new CancellationTokenSource();
            return _cancelTokenRun;

        }

        public async void StartArrayExperiments(object sender, EventArgs e)
        {
            _cancelTokenRun = ResetCancelToken();
            Task task = Task.Run(async () =>
            {
                await RunCommontExperements(ArrayTestConfig.SelectedAlgorithm,
                                                   ArrayTestConfig.ArraySize,
                                                   ArrayTestConfig.Iterations,
                                                   ArrayTestConfig.ArrayGenerator,
                                                   _cancelTokenRun.Token);
            }, _cancelTokenRun.Token);

            await task;
        }

        public async void StartMatrixExperiments(object sender, EventArgs e)
        {

            _cancelTokenRun = ResetCancelToken();

            Task task = Task.Run(async () =>
            {
                await RunCommontExperements(new Multiplication<int>(),
                    MatrixTestConfig.MatrixSize,
                    MatrixTestConfig.Iterations,
                    MatrixTestConfig.MatrixGenerator,
                    _cancelTokenRun.Token);

            }, _cancelTokenRun.Token);

            await task;

        }

        public async void StartPowExperiments(object sender, EventArgs e)
        {

            _cancelTokenRun = ResetCancelToken();

            Task task = Task.Run(async () =>
            {
                PowTestConfig.PowGenerator = new IntPowGenerator(PowTestConfig.X);
                await RunCommontExperements(PowTestConfig.SelectedAlgorithm,
                    PowTestConfig.MaxN,
                    PowTestConfig.Iterations,
                    PowTestConfig.PowGenerator,
                    _cancelTokenRun.Token);
            }, _cancelTokenRun.Token);

            await task;
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            _cancelTokenRun?.Cancel();
        }

    }
}