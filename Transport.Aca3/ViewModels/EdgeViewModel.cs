using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Transport.Aca3.ViewModels
{
    public class EdgeViewModel : VisualItemViewModel
    {
        public Color Color { get; set; } = Colors.Black;
        public double Thickness { get; set; } = 1.0;
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }
    }
}
