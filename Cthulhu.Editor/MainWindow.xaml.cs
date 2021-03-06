﻿using Cthulhu.Serialization;
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
        private const double ScaleStep = 1.25;

        private readonly TranslateTransform _mapTranslation = new TranslateTransform();
        private readonly ScaleTransform _mapScale = new ScaleTransform();
        private readonly Border _border = new Border();
        private readonly WorldInfo _worldInfo;
        
        private Point _mapPosition = default;
        private Point? _panStart = default;
        private int _zoomStep = 0;
        private double _mapScaleFactor = 1;
        private World _world = default;

        public MainWindow(WorldInfo worldInfo)
        {
            _worldInfo = worldInfo;
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
            int width = 300;
            int height = 300;
            int rawStride = (width * pixelFormat.BitsPerPixel + 7) / 8;
            var bytes = new byte[rawStride * height];
            var random = new Random();
            random.NextBytes(bytes);
            var bitmap = BitmapSource.Create(width, height, 96, 96, pixelFormat, null, bytes, rawStride);
            var image = new Image();
            image.Width = width;
            image.Source = bitmap;
            image.ClipToBounds = true;
            
            _border.Child = image;
            _border.Width = image.Width;
            _border.Height = image.Height;

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(_mapTranslation);
            transformGroup.Children.Add(_mapScale);
            _border.RenderTransform = transformGroup;
            
            worldCanvas.Children.Add(_border);
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
            var worldBytes = await File.ReadAllBytesAsync(worldFile);
            statusBarText.Text = $"Loaded {worldBytes.Length} bytes in {stopwatch.ElapsedMilliseconds}ms. Parsing world...";
            stopwatch.Restart();
            _world = await Task.Run(() => World.FromWorldData(worldBytes));
            statusBarText.Text = $"Loaded {_world.Name} in {stopwatch.ElapsedMilliseconds}ms. Generating bitmap...";
            stopwatch.Restart();
            
            var pixelFormat = PixelFormats.Bgr24;
            int width = _world.WorldWidthInTiles;
            int height = _world.WorldHeightInTiles;
            int rawStride = (width * pixelFormat.BitsPerPixel + 7) / 8;
            var imageBytes = await Task.Run(() =>
            {
                var bytes = new byte[rawStride * height];

                foreach (var pair in _world.Tiles)
                {
                    var tile = pair.Value;
                    var color = default(Color24);
                    
                    if (tile.IsActive || tile.HasActuator)
                    {
                        var tileInfo = _worldInfo.FindTileInfo(tile.TileType, tile.TextureU, tile.TextureV);
                        color = tileInfo.Color.GetValueOrDefault(color);
                    }
                    else if (tile.HasLiquid)
                    {
                        if (tile.HasLava)
                            color = _worldInfo.LavaColor;
                        else if (tile.HasHoney)
                            color = _worldInfo.HoneyColor;
                        else
                            color = _worldInfo.WaterColor;
                    }
                    else if (tile.HasWall)
                    {
                        //color = _worldInfo.WallInfoById[tile.WallType].Color.Value;
                        if (_worldInfo.WallInfoById.TryGetValue(tile.WallType, out var wallInfo) && wallInfo.Color.HasValue)
                            color = wallInfo.Color.Value;
                    }
                    else
                    {
                        var y = pair.Key.Y;

                        if (y < _world.WorldSurfaceY)
                            color = _worldInfo.SkyColor;
                        else if (y < _world.HellLayerY)
                            color = _worldInfo.EarthColor;
                        else
                            color = _worldInfo.HellColor;
                    }

                    var position = pair.Key;
                    int index = position.Y * rawStride + position.X * 3;
                    bytes[index + 0] = color.B;
                    bytes[index + 1] = color.G;
                    bytes[index + 2] = color.R;
                }

                return bytes;
            });

            var bitmap = BitmapSource.Create(width, height, 96, 96, pixelFormat, null, imageBytes, rawStride);
            var image = new Image();
            image.Width = width;
            image.Source = bitmap;
            image.ClipToBounds = true;

            _border.Child = image;
            _border.Width = image.Width;
            _border.Height = image.Height;

            statusBarText.Text = $"Loaded bitmap in {stopwatch.ElapsedMilliseconds}ms.";

            worldComboBox.IsEnabled = true;
            refreshButton.IsEnabled = true;
        }

        private void WorldCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (!_panStart.HasValue)
                {
                    _panStart = e.GetPosition(worldCanvas);
                    worldCanvas.Cursor = Cursors.ScrollAll;
                }
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
                    _mapPosition += diff / _mapScaleFactor;
                    _panStart = default;
                    worldCanvas.Cursor = Cursors.Arrow;
                }
            }
        }

        private void WorldCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var here = e.GetPosition(worldCanvas);
            
            if (_panStart.HasValue)
            {
                var diff = here - _panStart.Value;
                var destination = diff / _mapScaleFactor + _mapPosition;
                _mapTranslation.X = destination.X;
                _mapTranslation.Y = destination.Y;
            }
            else if (_world != null)
            {
                var mapCoordinates = (Vector)here / _mapScaleFactor - (Vector)_mapPosition;
                var tileCoordinates = new Point32((int)mapCoordinates.X, (int)mapCoordinates.Y);

                if (0 <= tileCoordinates.X &&
                    tileCoordinates.X < _world.WorldWidthInTiles &&
                    0 <= tileCoordinates.Y &&
                    tileCoordinates.Y < _world.WorldHeightInTiles)
                {
                    var tileText = "no tile";
                    var tile = _world.GetTile(tileCoordinates.X, tileCoordinates.Y);

                    if (tile.IsActive || tile.HasActuator)
                    {
                        var tileInfo = _worldInfo.FindTileInfo(tile.TileType, tile.TextureU, tile.TextureV);

                        if (tileInfo != null)
                            tileText = tileInfo.Name;
                    }

                    var liquidText = "no liquid";
                    if (tile.HasLiquid)
                    {
                        if (tile.HasLava)
                            liquidText = "Lava";
                        else if (tile.HasHoney)
                            liquidText = "Honey";
                        else
                            liquidText = "Water";
                    }

                    var wallText = "no wall";
                    if (tile.HasWall && _worldInfo.WallInfoById.TryGetValue(tile.WallType, out var wallInfo))
                        wallText = wallInfo.Name;

                    var gps = _world.GetGpsPosition(tileCoordinates);
                    var xText = gps.X < 0 ? $"{-gps.X} feet west" : $"{gps.X} feet east";
                    var yText = gps.Y < 0 ? $"{-gps.Y} feet above" : $"{gps.Y} feet below";
                    statusBarText.Text = $"{tileCoordinates} ({xText} {yText}) {tileText} - {liquidText} - {wallText}";
                }
            }
        }

        private void WorldCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!_panStart.HasValue) // No zooming while panning.
            {
                var here = e.GetPosition(worldCanvas);
                var absoluteHere = (Vector)here / _mapScaleFactor - (Vector)_mapPosition;

                if (e.Delta > 0)
                {
                    ++_zoomStep;
                }
                else
                {
                    --_zoomStep;
                }

                _mapScaleFactor = Math.Pow(ScaleStep, _zoomStep);
                _mapScale.ScaleX = _mapScaleFactor;
                _mapScale.ScaleY = _mapScaleFactor;

                _mapPosition = (Point)((Vector)here / _mapScaleFactor - absoluteHere);
                _mapTranslation.X = _mapPosition.X;
                _mapTranslation.Y = _mapPosition.Y;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Home && !_panStart.HasValue)
            {
                _mapPosition = default;
                _mapTranslation.X = 0;
                _mapTranslation.Y = 0;
                _zoomStep = 0;
                _mapScaleFactor = 1;
                _mapScale.ScaleX = 1;
                _mapScale.ScaleY = 1;
            }
        }
    }
}
