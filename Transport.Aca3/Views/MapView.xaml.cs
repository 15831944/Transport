using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Transport.Aca3.ViewModels;
using Transport.DataAccessLayer;

namespace Transport.Aca3.Views
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();
        }

        //private void BuildGraph_OnClick(object sender, RoutedEventArgs e)
        //{
        //    using (var db = new TransportContext())
        //    {
        //        foreach (var dbArea in db.Areas)
        //        {
        //            var id = dbArea.AreaId;
        //            // TODO: заменить на вызов одного метода с массивом в аргументах
        //            WebBrowser.InvokeScript("DrawNode", id, id.ToString(), "#FFFF0000", dbArea.Location.Latitude, dbArea.Location.Longitude);
        //        }

        //        var rand = new Random();

        //        foreach (var areaRoute in db.AreaRoutes)
        //        {
        //            var id = rand.Next();
        //            var p1 = areaRoute.Origin.Location;
        //            var p2 = areaRoute.Destination.Location;
        //            // TODO: заменить на вызов одного метода с массивом в аргументах
        //            WebBrowser.InvokeScript("DrawEdge", id, p1.Latitude, p1.Longitude, p2.Latitude, p2.Longitude, "#000000", 1);
        //        }
        //    }

        //    WebBrowser.Visibility = Visibility.Visible;
        //}
    }
}
