using Lab_1.ArrayAlgorithms;
using Lab_1.Generators;
using Lab_1.MatrixAlgorithms;
using Lab_1.Tests;
using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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


        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task RunPlotUpdate(WpfPlot plot, int refreshRate, CancellationToken token = default)
        {
            plot.Plot.Axes.SetLimitsX(0, ArrayTestConfig.ArraySize + 200);
            plot.Plot.Axes.SetLimitsY(0, 5000);

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(refreshRate, CancellationToken.None);
                //plot.Plot.Axes.SetLimitsX(0, _maxSize + 200);
                //plot.Plot.Axes.SetLimitsY(0, 5000);
                plot.Refresh();
                MainPlot.Plot.Axes.AutoScale();

            }
        }


        private async Task RunCommontExperements<T>(IAlgorithm<T> algorithm, int maxSize, int iterations, IGenerator<T> generator, CancellationToken token = default)
        {
            TestManager manager = new();
            List<Coordinates> source = [];
            MainPlot.Plot.Add.Scatter(new ScatterSourceCoordinatesList(source));
            MainPlot.Plot.Axes.SetLimitsX(0, maxSize + 200);
            MainPlot.Plot.Axes.SetLimitsY(0, 5000);
            CancellationTokenSource cancelTokenSourceUpdate = new();
            var cancelTokenUpdate = cancelTokenSourceUpdate.Token;
            _ = Task.Run(async () => await RunPlotUpdate(MainPlot, 100, cancelTokenUpdate), cancelTokenUpdate);
            int n = 0;
            await foreach (Point point in manager.TestAlgorithm(
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
                await foreach (Point point in manager.TestAlgorithm(
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
        private async Task RunCommontExperements<T, K>(IAlgorithm<T, Task<K>> algorithm, int maxSize, int iterations, IGenerator<T> generator, CancellationToken token = default)
        {
            TestManager manager = new();
            List<Coordinates> source = [];
            MainPlot.Plot.Add.Scatter(new ScatterSourceCoordinatesList(source));
            MainPlot.Plot.Axes.SetLimitsX(0, maxSize + 200);
            MainPlot.Plot.Axes.SetLimitsY(0, 5000);
            CancellationTokenSource cancelTokenSourceUpdate = new();
            var cancelTokenUpdate = cancelTokenSourceUpdate.Token;
            _ = Task.Run(async () => await RunPlotUpdate(MainPlot, 100, cancelTokenUpdate), cancelTokenUpdate);
            int n = 0;
            await foreach (Point point in manager.TestAlgorithm(
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
                await foreach (Point point in manager.TestAlgorithm(
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