using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NAllo.Arche.UI.Views.Common
{
    public class FreeFormContentControl : ContentControl
    {
        public Geometry FormGeometry
        {
            get { return (Geometry) GetValue(FormGeometryProperty); }
            set { SetValue(FormGeometryProperty, value); }
        }

        public static readonly DependencyProperty FormGeometryProperty =
            DependencyProperty.Register("FormGeometry", typeof (Geometry), typeof (FreeFormContentControl), new UIPropertyMetadata(null));

        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection) GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(FreeFormContentControl), new UIPropertyMetadata(null));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(FreeFormContentControl), new UIPropertyMetadata(null));

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(FreeFormContentControl), new UIPropertyMetadata(null));

        static FreeFormContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof (FreeFormContentControl),
                new FrameworkPropertyMetadata(typeof (FreeFormContentControl)));
        }
    }
}
