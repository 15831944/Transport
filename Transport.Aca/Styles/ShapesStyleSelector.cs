using System.Windows;
using System.Windows.Controls;
using Transport.Aca.Model;

namespace Transport.Aca.Styles
{
    public class ShapesStyleSelector : StyleSelector
    {
        public Style NodeStyle { get; set; }
        public Style EdgeStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is Node)
            {
                return NodeStyle;
            }
            if (item is Edge)
            {
                return EdgeStyle;
            }

            return base.SelectStyle(item, container);
        }
    }
}
