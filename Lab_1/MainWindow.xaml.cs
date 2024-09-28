using Lab_1.ArrayAlgorithms;
using Lab_1.Generators;
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

        private async Task RunArrayAlgorithmExperements(IArrayAlgorithm<int> algorithm, int maxSize, int iterations, IArrayGenerator<int> arrayGenerator, CancellationToken token = default)
        {
            TestManager manager = new();
            List<Coordinates> source = [];
            MainPlot.Plot.Add.Scatter(new ScatterSourceCoordinatesList(source));
            MainPlot.Plot.Axes.SetLimitsX(0, maxSize + 200);
            MainPlot.Plot.Axes.SetLimitsY(0, 5000);
            CancellationTokenSource cancelTokenSourceUpdate = new();
            var cancelTokenUpdate = cancelTokenSourceUpdate.Token;
            _ = Task.Run(async () => await RunPlotUpdate(MainPlot, 100, cancelTokenUpdate), cancelTokenUpdate);


            Point prev = new(0, 0);

            int n = 0;
            await foreach (Point point in manager.TestArrayAlgorithm(
                algorithm,
                arrayGenerator,
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
                await foreach (Point point in manager.TestArrayAlgorithm(
                    algorithm,
                    arrayGenerator,
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

        public async void Start(object sender, EventArgs e)
        {
            if (_cancelTokenRun != null)
            {
                _cancelTokenRun.Cancel();
                _cancelTokenRun.Dispose();
                _cancelTokenRun = null;
                MainPlot.Plot.Clear();
            }
            _cancelTokenRun = new CancellationTokenSource();
            var cancelTokenRun = _cancelTokenRun.Token;
            Task task = Task.Run(async () =>
            {
                await RunArrayAlgorithmExperements(ArrayTestConfig.SelectedAlgorithm,
                                                   ArrayTestConfig.ArraySize,
                                                   ArrayTestConfig.Iterations,
                                                   ArrayTestConfig.ArrayGenerator,
                                                   cancelTokenRun);
            }, _cancelTokenRun.Token);

            await task;
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            _cancelTokenRun?.Cancel();
        }

    }
}