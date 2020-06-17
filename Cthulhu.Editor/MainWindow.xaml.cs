using Cthulhu.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cthulhu.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TranslateTransform _mapTranslation = new TranslateTransform();
        private readonly ScaleTransform _mapScale = new ScaleTransform();
        
        private Point _mapPosition = default;
        private Point? _panStart = default;
        private int _zoomStep = 0;
        private double _scale = 1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task<World> LoadWorldAsync(string path)
        {
            var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
            var world = World.FromWorldData(bytes);
            return world;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWorldList();
            worldCanvas.Background = Brushes.Blue;

            var pixelFormat = PixelFormats.Bgr24;
            int width = 200;
            int height = 200;
            int rawStride = (width * pixelFormat.BitsPerPixel + 7) / 8;
            var bytes = new byte[rawStride * height];
            var random = new Random();
            random.NextBytes(bytes);
            var bitmap = BitmapSource.Create(width, height, 96, 96, pixelFormat, null, bytes, rawStride);
            var image = new Image();
            image.Width = width;
            image.Source = bitmap;
            image.ClipToBounds = true;
            
            var border = new Border();
            border.Child = image;
            border.Width = image.Width;
            border.Height = image.Height;

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(_mapTranslation);
            transformGroup.Children.Add(_mapScale);
            border.RenderTransform = transformGroup;
            
            worldCanvas.Children.Add(border);
        }

        private void LoadWorldList()
        {
            var worldFolder = World.GetFolder();
            var worldFiles = Directory.GetFiles(worldFolder, "*.wld");
            var items = worldComboBox.Items;
            items.Clear();
            
            foreach (var worldFile in worldFiles)
            {
                var item = new ComboBoxItem
                {
                    Content = System.IO.Path.GetFileName(worldFile),
                    Tag = worldFile
                };

                items.Add(item);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadWorldList();
        }

        private async void WorldComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (ComboBoxItem)worldComboBox.SelectedItem;

            if (selected != null)
            {
                worldComboBox.IsEnabled = false;
                refreshButton.IsEnabled = false;
                var worldFileName = (string)selected.Content;
                var worldFile = (string)selected.Tag;
                await LoadWorldAsync(worldFile, worldFileName);
            }
        }

        private async Task LoadWorldAsync(string worldFile, string worldFileName)
        {
            statusBarText.Text = "Loading " + worldFileName;
            var stopwatch = Stopwatch.StartNew();
            var bytes = await File.ReadAllBytesAsync(worldFile);
            statusBarText.Text = $"Loaded {bytes.Length} bytes in {stopwatch.ElapsedMilliseconds}ms. Parsing world...";
            stopwatch.Restart();
            var world = await Task.Run(() => World.FromWorldData(bytes));
            statusBarText.Text = $"Loaded {world.Name} in {stopwatch.ElapsedMilliseconds}ms.";
            worldComboBox.IsEnabled = true;
            refreshButton.IsEnabled = true;
        }

        private void WorldCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _panStart = e.GetPosition(worldCanvas);
            }
        }

        private void WorldCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_panStart.HasValue)
                {
                    var here = e.GetPosition(worldCanvas);
                    var diff = here - _panStart.Value;
                    _mapPosition += diff / _scale;
                    _panStart = default;
                }
            }
        }

        private void WorldCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_panStart.HasValue)
            {
                var here = e.GetPosition(worldCanvas);
                var diff = here - _panStart.Value;
                _mapTranslation.X = _mapPosition.X + diff.X / _scale;
                _mapTranslation.Y = _mapPosition.Y + diff.Y / _scale;
            }
        }

        private void WorldCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!_panStart.HasValue) // No zooming while panning.
            {
                if (e.Delta > 0)
                {
                    ++_zoomStep;
                }
                else
                {
                    --_zoomStep;
                }

                _scale = Math.Pow(1.25, _zoomStep);
                _mapScale.ScaleX = _scale;
                _mapScale.ScaleY = _scale;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Home)
            {
                _panStart = default;
                _mapPosition = default;
                _mapTranslation.X = 0;
                _mapTranslation.Y = 0;
                _scale = 1;
                _mapScale.ScaleX = 1;
                _mapScale.ScaleY = 1;
            }
        }
    }
}
