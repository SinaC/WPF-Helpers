using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CustomControls
{
    //  ----|-------- 
    // | *********** |<--Path
    // | **Content** |
    // | *********** |
    //  ------------- 
    [ContentProperty("Content")]
    public class BalloonContentControl : FrameworkElement
    {
        private readonly System.Globalization.CultureInfo _stringFormatCulture;

        private readonly SimplePanel _panel; // Host ContentPanel and Path
        private readonly SimplePanel _contentPanel; // host Content with a margin added to put Content inside Path
        private readonly List<object> _logicalChildren; // contains Content (only real children)
        private readonly Path _path; // Path definition

        #region Position

        // TODO: Left, Right
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Dock), typeof(BalloonContentControl), new PropertyMetadata(Dock.Top, PositionChanged));
        
        private static void PositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonContentControl control = d as BalloonContentControl;

            if (control == null)
                return;

            control.UpdatePath();
            control.UpdateContentMargin();
        }

        public Dock Position
        {
            get { return (Dock)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        #endregion

        #region CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                "CornerRadius",
                typeof (double),
                typeof (BalloonContentControl),
                new PropertyMetadata(2.0, CornerRadiusChanged));

        private static void CornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonContentControl control = d as BalloonContentControl;

            if (control == null)
                return;

            control.UpdateContentMargin();
            control.UpdatePath();
        }

        public double CornerRadius
        {
            get { return (double) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        #endregion

        #region ArrowSize

        public static readonly DependencyProperty ArrowSizeProperty =
            DependencyProperty.Register(
                "ArrowSize",
                typeof (double),
                typeof (BalloonContentControl),
                new PropertyMetadata(5.0, ArrowSizeChanged));

        private static void ArrowSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonContentControl control = d as BalloonContentControl;

            if (control == null)
                return;

            control.UpdateContentMargin();
            control.UpdatePath();
        }

        public double ArrowSize
        {
            get { return (double) GetValue(ArrowSizeProperty); }
            set { SetValue(ArrowSizeProperty, value); }
        }

        #endregion

        #region ArrowPosition

        public static readonly DependencyProperty ArrowPositionProperty =
            DependencyProperty.Register(
                "ArrowPosition",
                typeof(double),
                typeof(BalloonContentControl),
                new PropertyMetadata(0.3, ArrowPositionChanged));

        private static void ArrowPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonContentControl control = d as BalloonContentControl;

            if (control == null)
                return;

            control.UpdateContentMargin();
            control.UpdatePath();
        }

        public double ArrowPosition
        {
            get { return (double)GetValue(ArrowPositionProperty); }
            set { SetValue(ArrowPositionProperty, Math.Min(Math.Max(0, value), 1.0)); }
        }

        #endregion

        #region Stroke

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke", 
            typeof (Brush), 
            typeof (BalloonContentControl), 
            new PropertyMetadata(new SolidColorBrush(Colors.Red), StrokeChanged));
        
        private static void StrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonContentControl control = d as BalloonContentControl;

            if (control == null || control._path == null)
                return;

            control._path.Stroke = control.Stroke;
        }

        public Brush Stroke
        {
            get { return (Brush) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        #endregion

        #region StrokeThickness

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", 
            typeof (double), 
            typeof (BalloonContentControl), 
            new PropertyMetadata(1.0, StrokeThicknessChanged));

        private static void StrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonContentControl control = d as BalloonContentControl;

            if (control == null || control._path == null)
                return;

            control._path.StrokeThickness = control.StrokeThickness;
        }

        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        #endregion

        #region Fill

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill",
            typeof(Brush),
            typeof(BalloonContentControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent), FillChanged));

        private static void FillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonContentControl control = d as BalloonContentControl;

            if (control == null || control._path == null)
                return;

            control._path.Fill = control.Fill;
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        #endregion

        #region Content

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof (object),
                typeof (BalloonContentControl),
                new UIPropertyMetadata(null, OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BalloonContentControl control = d as BalloonContentControl;

            if (control == null)
                return;

            if (e.OldValue != null)
            {
                control._logicalChildren.Remove(e.OldValue);
                UIElement element = e.OldValue as UIElement;
                if (element != null)
                    control._contentPanel.Children.Remove(element);
            }
            if (e.NewValue != null)
            {
                control._logicalChildren.Add(e.NewValue);
                UIElement element = e.NewValue as UIElement;
                if (element != null)
                    control._contentPanel.Children.Add(element);
            }
        }

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        #endregion

        public BalloonContentControl()
        {
            _stringFormatCulture = (System.Globalization.CultureInfo) System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            _stringFormatCulture.NumberFormat.NumberDecimalSeparator = ".";

            // Host Content + margin
            _contentPanel = new SimplePanel();
            UpdateContentMargin();

            // Path
            _path = new Path
                {
                    Data = Geometry.Parse("M0,0 L40,0 L40,40 L0,40 Z"),
                    StrokeThickness = StrokeThickness,
                    Stroke = Stroke,
                    StrokeLineJoin = PenLineJoin.Miter,
                    Fill = Fill,
                };

            // Host _contentPanel and _path 
            _panel = new SimplePanel();
            _panel.Children.Add(_path);
            _panel.Children.Add(_contentPanel);
            _panel.SizeChanged += PanelOnSizeChanged;
            //
            AddVisualChild(_panel); // only one visual child

            //
            _logicalChildren = new List<object>();
        }

        private void UpdateContentMargin()
        {
            if (Position == Dock.Top)
                _contentPanel.Margin = new Thickness(CornerRadius, ArrowSize + CornerRadius, CornerRadius, CornerRadius);
            else if (Position == Dock.Bottom)
                _contentPanel.Margin = new Thickness(CornerRadius, CornerRadius, CornerRadius, CornerRadius + ArrowSize);
        }

        private void UpdatePath()
        {
            // Set Path.Data
            double width = ActualWidth;
            double height = ActualHeight - StrokeThickness; // TODO: why substracting StrokeThickness ???
            height = Math.Max(height, _contentPanel.ActualHeight); // height cannot be smaller than content
            string data = null;
            if (Position == Dock.Top)
                //                   "M0,7   A2,2 90 0 1 2,5         L220,5   L225,0 L230,5   L389,5   A2,2 90 0 1 391,7       L391,66  A2,2 90 0 1 389,68      L2,68    A2,2 90 0 1 0,66 Z");
                data = String.Format(_stringFormatCulture,
                                     "M0,{9} A{7},{7} 90 0 1 {7},{8} L{0},{8} L{1},0 L{2},{8} L{3},{8} A{7},{7} 90 0 1 {4},{9} L{4},{5} A{7},{7} 90 0 1 {3},{6} L{7},{6} A{7},{7} 90 0 1 0,{5} Z",
                                     width*ArrowPosition - ArrowSize, 
                                     width*ArrowPosition, 
                                     width*ArrowPosition + ArrowSize, 
                                     width - CornerRadius, 
                                     width, 
                                     height - CornerRadius, 
                                     height,
                                     CornerRadius,
                                     ArrowSize,
                                     CornerRadius + ArrowSize);
            else if (Position == Dock.Bottom)
                data = String.Format(_stringFormatCulture,
                                     "M0,{0} A{0},{0} 90 0 1 {0},0 L{1},0 A{0},{0} 90 0 1 {2},{0} L{2},{3} A{0},{0} 90 0 1 {1},{4} L{5},{4} L{6},{7} L{8},{4} L{0},{4} A{0},{0} 90 0 1 0,{3} Z",
                                     CornerRadius,
                                     width-CornerRadius,
                                     width,
                                     height-ArrowSize-CornerRadius,
                                     height-ArrowSize,
                                     width*ArrowPosition+ArrowSize,
                                     width*ArrowPosition,
                                     height,
                                     width*ArrowPosition-ArrowSize);
            _path.Data = Geometry.Parse(data);
        }

        private void PanelOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            UpdatePath();
        }

        #region FrameworkElement overrides

        //These methods are required as a bare minimum for making a custom
        //FrameworkElement render correctly. These methods get the objects which
        //make up the visual and logical tree (the AddVisualChild and AddLogicalChild
        //methods only setup the relationship between the parent/child objects).

        //The Arrange and Measure methods simply delegate to the grid which
        //calculates where any content should be placed.

        protected override IEnumerator LogicalChildren
        {
            get { return _logicalChildren.GetEnumerator(); }
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index > 1)
                throw new ArgumentOutOfRangeException("index");

            return _panel;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _panel.Measure(availableSize);
            return _panel.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _panel.Arrange(new Rect(finalSize));
            return finalSize;
        }

        #endregion

        #region Simple panel

        private class SimplePanel : Panel
        {
            protected override Size MeasureOverride(Size availableSize)
            {
                Size resultSize = new Size(0, 0);

                foreach (UIElement child in Children)
                {
                    child.Measure(availableSize);
                    resultSize.Width = Math.Max(resultSize.Width, child.DesiredSize.Width);
                    resultSize.Height = Math.Max(resultSize.Height, child.DesiredSize.Height);
                }

                return resultSize;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                foreach (UIElement child in InternalChildren)
                    child.Arrange(new Rect(finalSize));

                return finalSize;
            }
        }

        #endregion
    }
}
