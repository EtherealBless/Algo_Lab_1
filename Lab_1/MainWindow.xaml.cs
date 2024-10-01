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
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows;
using static Lab_1.Approximation.Approximator;
using Point = Lab_1.Utils.Point;

namespace Lab_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ArrayTestConfig ArrayTestConfig { get; set; } = new();
        public MatrixTestConfig MatrixTestConfig { get; set; } = new();

        public PowTestConfig PowTestConfig { get; set; } = new();

        private CancellationTokenSource? _cancelTokenRun;

        private List<double>? _approximationParameters = null;
              public event PropertyChangedEventHandler PropertyChanged;

        private static FunctionPlot? FunctionPlot { get; set; }

        private string _functionName = "";
        public string FunctionName
        {
            get
            {
                return _functionName;
            }
            set
            {
                _functionName = value;
                NotifyPropertyChanged();
            }
        }
        private bool TEST = false;
        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

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
            _approximationParameters = null;
            plot.Plot.Axes.SetLimitsX(0, 200);
            plot.Plot.Axes.SetLimitsY(0, 200);
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
                    FunctionPlot = DrawApproximation(algorithm, source, FunctionPlot);
                    FunctionPlot.MaxX = source.Count * 1.1;

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
            MainPlot.Plot.Clear();
            MainPlot.Plot.Add.Scatter(new ScatterSourceCoordinatesList(source));

            FunctionPlot? functionPlot = null;

            CancellationTokenSource cancelTokenSourceUpdate = new();
            var cancelTokenUpdate = cancelTokenSourceUpdate.Token;

            _ = Task.Run(async () => await RunPlotUpdate(MainPlot, 100, algorithm, source, cancelTokenUpdate), cancelTokenUpdate);

            int n = 0;
            await foreach (var point in manager.TestAlgorithm(
                algorithm,
                generator,
                maxSize,
                _cancelTokenRun?.Token))
            {
                if (token.IsCancellationRequested)
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
                    maxSize,
                    _cancelTokenRun?.Token))
                {
                    if (token.IsCancellationRequested)
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
            _approximationParameters = null;
            lock (MainPlot.Plot)
            {
                if (FunctionPlot != null)
                    MainPlot.Plot.Remove(FunctionPlot);
            }
        }

        private FunctionPlot DrawApproximation<T, K>(IAlgorithm<T, K> algorithm, List<Coordinates> source, FunctionPlot? functionPlot)
        {
            DoubleVector res;
            if (_approximationParameters == null)
            {
                res = ApproximateCoord(algorithm, source);
            }
            else
            {
                res = ApproximateCoord(algorithm, source, new DoubleVector(_approximationParameters.ToArray()));
            }
            _approximationParameters = res.ToList();
            var approximationFunction = AlgorithmsDifficulty[algorithm.GetType()] ?? throw new ArgumentException("Wrong algorithm type");
            if (functionPlot != null)
            {
                ClearApproximation();
            }
            FunctionName = string.Format(AlgorithmsDifficulty[algorithm.GetType()].Formula, (Object[])[.. res]);

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
            MainPlot.Plot.Clear();

            MainPlot.Plot.Add.Scatter(new ScatterSourceCoordinatesList(source));

            CancellationTokenSource cancelTokenSourceUpdate = new();
            var cancelTokenUpdate = cancelTokenSourceUpdate.Token;
            _ = Task.Run(async () => await RunPlotUpdate(MainPlot, 100, algorithm, source, cancelTokenUpdate), cancelTokenUpdate);
            int n = 0;
            await foreach (var point in manager.TestAlgorithm(
                algorithm,
                generator,
                maxSize,
                _cancelTokenRun?.Token ?? default))
            {
                if (token.IsCancellationRequested)
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
                maxSize,
                _cancelTokenRun?.Token))
                {
                    if (token.IsCancellationRequested)
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
            }
            _cancelTokenRun = new CancellationTokenSource();
            _cancelTokenRun.Token.ThrowIfCancellationRequested();
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