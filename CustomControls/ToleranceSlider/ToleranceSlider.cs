using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CustomControls.ToleranceSlider
{
    // http://msdn.microsoft.com/en-us/library/ee330302(v=vs.110).aspx
    //http://referencesource.microsoft.com/#PresentationFramework/Framework/System/Windows/Controls/Slider.cs,6532ecbe410bb4ae

    // Minimum <= MinimumWithoutTolerance < MaximumWithoutTolerance <= Maximum
    // Minimum and Maximum are computed with tolerance
    // Minimum -> MinimumWithoutTolerance and MaximumWithoutTolerance -> Maximum may have a different background than MinimumWithoutTolerance -> MaximumWithoutTolerance
    public class ToleranceSlider : Slider
    {
        private static readonly Color DefaultToleranceColor = Colors.Pink;
        private static readonly Color DefaultNoToleranceColor = Colors.LightBlue;

        private const string TrackName = "PART_Track";
        private const string MiddleOuterBarName = "PART_MiddleOuterBar";
        private const string MiddleInnerBarName = "PART_MiddleInnerBar";
        
        private Track _track;
        private Border _middleOuterBar;
        private Border _middleInnerBar;

        static ToleranceSlider()
        {
            //http://stackoverflow.com/questions/1237611/creating-default-style-for-custom-control
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToleranceSlider), new FrameworkPropertyMetadata(typeof(ToleranceSlider)));

            //http://msdn.microsoft.com/en-us/library/ms752375.aspx#scenarios
            //http://stackoverflow.com/questions/18209913/metadata-override-and-base-metadata-must-be-of-the-same-type
            MinimumProperty.OverrideMetadata(typeof(ToleranceSlider), new FrameworkPropertyMetadata(default(double), UpdateLayoutCallback));
            MaximumProperty.OverrideMetadata(typeof(ToleranceSlider), new FrameworkPropertyMetadata(default(double), UpdateLayoutCallback));
        }

        //http://stackoverflow.com/questions/2720104/accessing-wpf-template-for-custom-control-from-code-behind
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _track = GetTemplateChild(TrackName) as Track;
            _middleOuterBar = GetTemplateChild(MiddleOuterBarName) as Border;
            _middleInnerBar = GetTemplateChild(MiddleInnerBarName) as Border;

            foreach (ToleranceTickBar toleranceTickBar in FindVisualChildren<ToleranceTickBar>(this))
                toleranceTickBar.OnPreApplyTemplate();
        }

        //http://stackoverflow.com/questions/7801680/how-can-i-manually-tell-an-owner-drawn-wpf-control-to-refresh-without-executing
        public static readonly DependencyProperty MinimumWithoutToleranceProperty =
            DependencyProperty.Register(
                "MinimumWithoutTolerance",
                typeof(double),
                typeof(ToleranceSlider),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender, UpdateLayoutCallback));

        public double MinimumWithoutTolerance
        {
            get { return (double)GetValue(MinimumWithoutToleranceProperty); }
            set { SetValue(MinimumWithoutToleranceProperty, value); }
        }

        public static readonly DependencyProperty MaximumWithoutToleranceProperty =
            DependencyProperty.Register(
                "MaximumWithoutTolerance",
                typeof(double),
                typeof(ToleranceSlider),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender, UpdateLayoutCallback));

        public double MaximumWithoutTolerance
        {
            get { return (double)GetValue(MaximumWithoutToleranceProperty); }
            set { SetValue(MaximumWithoutToleranceProperty, value); }
        }

        public static readonly DependencyProperty ToleranceColorProperty =
            DependencyProperty.Register(
                "ToleranceColor",
                typeof(Color),
                typeof(ToleranceSlider),
                new FrameworkPropertyMetadata(DefaultToleranceColor, FrameworkPropertyMetadataOptions.AffectsRender, UpdateLayoutCallback));

        public Color ToleranceColor
        {
            get { return (Color)GetValue(ToleranceColorProperty); }
            set { SetValue(ToleranceColorProperty, value); }
        }


        public static readonly DependencyProperty NoToleranceColorProperty =
            DependencyProperty.Register(
                "NoToleranceColor",
                typeof(Color),
                typeof(ToleranceSlider),
                new FrameworkPropertyMetadata(DefaultNoToleranceColor, FrameworkPropertyMetadataOptions.AffectsRender, UpdateLayoutCallback));

        public Color NoToleranceColor
        {
            get { return (Color)GetValue(NoToleranceColorProperty); }
            set { SetValue(NoToleranceColorProperty, value); }
        }

        private static void UpdateLayoutCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ToleranceSlider slider = dependencyObject as ToleranceSlider;
            if (slider == null)
                return;
            slider.UpdateMiddleBarBackground();
        }

        private void UpdateMiddleBarBackground()
        {
            if (_middleOuterBar == null)
                return;
            _middleOuterBar.Background = new SolidColorBrush(ToleranceColor);

            if (Maximum <= Minimum)
                return;

            // Clamp values to avoid weird cases
            double minWithoutTolerance = MinimumWithoutTolerance;
            double maxWithoutTolerance = MaximumWithoutTolerance;
            if (minWithoutTolerance < Minimum)
                minWithoutTolerance = Minimum;
            if (maxWithoutTolerance > Maximum)
                maxWithoutTolerance = Maximum;
            double reservedSpace = _track == null || _track.Thumb == null ? 0 : _track.Thumb.ActualWidth;

            // Use outer bar to compute start and end because its width is never modified while change min/max
            double range = Maximum - Minimum;
            double startX = (minWithoutTolerance - Minimum) * (_middleOuterBar.ActualWidth - reservedSpace) / range;
            double endX = (maxWithoutTolerance - Minimum) * (_middleOuterBar.ActualWidth - reservedSpace) / range + reservedSpace;

            if (_middleInnerBar != null)
            {
                _middleInnerBar.Background = new SolidColorBrush(NoToleranceColor);
                _middleInnerBar.HorizontalAlignment = HorizontalAlignment.Stretch;
                _middleInnerBar.Margin = new Thickness(startX, 0, _middleOuterBar.ActualWidth - endX, 0);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            UpdateMiddleBarBackground();
        }
        internal static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                    if (child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
        }
    }
}
