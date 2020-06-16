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
    }
}
