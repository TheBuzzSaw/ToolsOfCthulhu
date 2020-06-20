using Cthulhu.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cthulhu.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var xml = File.ReadAllText("tiles.xml");
            var worldInfo = WorldInfo.FromWorldInfoData(xml);
            var mainWindow = new MainWindow(worldInfo);
            mainWindow.Show();
        }
    }
}
