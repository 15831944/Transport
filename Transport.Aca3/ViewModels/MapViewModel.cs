using System;
using GalaSoft.MvvmLight;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using Transport.Aca3.Services;

namespace Transport.Aca3.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        #region Consts

        private const string MapHtmlResourcePath = "Transport.Aca3.GoogleMap.html";

        #endregion

        #region Fields

        private readonly IDataAccessService _dataAccessService;

        #endregion Fields

        #region Constructors

        public MapViewModel(IDataAccessService dataAccessService)
        {
            _dataAccessService = dataAccessService;

            // Создать браузер и загрузить содержимое страницы
            WebBrowser = new WebBrowser();
            var html = LoadMapHtmlResource();
            WebBrowser.NavigateToString(html);
            WebBrowser.LoadCompleted += (sender, args) => { DrawNetworkFromDb(); };
        }

        #endregion Constructors

        #region Methods

        public void DrawNetworkFromDb()
        {
            WebBrowser.InvokeScript("InitMap");

            var rand = new Random();

            foreach (var node in _dataAccessService.GetGraphNodes())
            {
                var nodeId = node.Id;
                // TODO: заменить на вызов одного метода с массивом в аргументах
                WebBrowser.InvokeScript("DrawNode", nodeId, nodeId.ToString(), "#FFFF0000", node.Lat, node.Lon);

                foreach (var edge in node.Edges)
                {
                    var edgeId = rand.Next();
                    var p1Lat = edge.Source.Lat;
                    var p1Lon = edge.Source.Lon;
                    var p2Lat = edge.Dest.Lat;
                    var p2Lon = edge.Dest.Lon;
                    
                    // TODO: заменить на вызов одного метода с массивом в аргументах
                    WebBrowser.InvokeScript("DrawEdge", edgeId, p1Lat, p1Lon, p2Lat, p2Lon, "#000000", 1);
                }
            }
        }

        private string LoadMapHtmlResource()
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            using (var stream = assembly.GetManifestResourceStream(MapHtmlResourcePath))
            {
                if (stream == null) return String.Empty;

                var streamReader = new StreamReader(stream);
                return streamReader.ReadToEnd();
            }
        }

        #endregion Methods


        #region Properties

        public WebBrowser WebBrowser { get; }

        #endregion
    }
}
