using System.Windows;
using System.Windows.Controls;

namespace Waf.DotNetPad.Presentation.Controls
{
    public static class IconInfo
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(object), typeof(IconInfo), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));


        [AttachedPropertyBrowsableForType(typeof(Control))]
        public static object GetIcon(DependencyObject obj)
        {
            return obj.GetValue(IconProperty);
        }

        public static void SetIcon(DependencyObject obj, object value)
        {
            obj.SetValue(IconProperty, value);
        }
    }
}
