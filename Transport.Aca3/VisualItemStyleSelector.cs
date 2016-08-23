using System.Windows;
using System.Windows.Controls;
using Transport.Aca3.ViewModels;

namespace Transport.Aca3
{
    public class VisualItemStyleSelector : StyleSelector
    {
        public Style NodeStyle { get; set; }
        public Style EdgeStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is NodeViewModel)
            {
                return NodeStyle;
            }
            if (item is EdgeViewModel)
            {
                return EdgeStyle;
            }

            return base.SelectStyle(item, container);
        }
    }
}
