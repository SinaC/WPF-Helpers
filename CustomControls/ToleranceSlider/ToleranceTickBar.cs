using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace CustomControls.ToleranceSlider
{
    //http://www.dotnetframework.org/default.aspx/4@0/4@0/DEVDIV_TFS/Dev10/Releases/RTMRel/wpf/src/Framework/System/Windows/Controls/Primitives/TickBar@cs/1305600/TickBar@cs
    //http://referencesource.microsoft.com/#PresentationFramework/Framework/System/Windows/Controls/Primitives/TickBar.cs,690
    public class ToleranceTickBar : TickBar
    {
        // TODO: find how to react on ToleranceSlider (TemplatedParent) MinimumWithoutTolerance/MaximumWithoutTolerance modification
        //http://stackoverflow.com/questions/7801680/how-can-i-manually-tell-an-owner-drawn-wpf-control-to-refresh-without-executing
        // http://msdn.microsoft.com/en-us/library/ms754002(v=vs.110).aspx

        public static readonly DependencyProperty MinimumWithoutToleranceProperty = ToleranceSlider.MinimumWithoutToleranceProperty.AddOwner(
            typeof(ToleranceTickBar), 
            new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender));

        public double MinimumWithoutTolerance
        {
            get { return (double)GetValue(MinimumWithoutToleranceProperty); }
            set { SetValue(MinimumWithoutToleranceProperty, value); }
        }

        public static readonly DependencyProperty MaximumWithoutToleranceProperty = ToleranceSlider.MaximumWithoutToleranceProperty.AddOwner(
            typeof(ToleranceTickBar), 
            new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender));

        public double MaximumWithoutTolerance
        {
            get { return (double)GetValue(MaximumWithoutToleranceProperty); }
            set { SetValue(MaximumWithoutToleranceProperty, value); }
        }

        //http://www.dotnetframework.org/default.aspx/4@0/4@0/DEVDIV_TFS/Dev10/Releases/RTMRel/wpf/src/Framework/System/Windows/Controls/Primitives/TickBar@cs/1305600/TickBar@cs
        public void OnPreApplyTemplate()
        {
            ToleranceSlider slider = TemplatedParent as ToleranceSlider;

            if (slider != null)
            {
                BindToTemplatedParent(MinimumWithoutToleranceProperty, ToleranceSlider.MinimumWithoutToleranceProperty);
                BindToTemplatedParent(MaximumWithoutToleranceProperty, ToleranceSlider.MaximumWithoutToleranceProperty);
            }
        }

        private void BindToTemplatedParent(DependencyProperty target, DependencyProperty source)
        {
            Binding binding = new Binding
            {
                RelativeSource = RelativeSource.TemplatedParent,
                Path = new PropertyPath(source)
            };
            SetBinding(target, binding);
        }

        private void DisplayMinAndMax(DrawingContext dc)
        {
            ToleranceSlider slider = TemplatedParent as ToleranceSlider;
            if (slider == null)
                return;

            Size size = new Size(ActualWidth, ActualHeight);

            //http://stackoverflow.com/questions/6865477/how-to-create-a-wrapping-button-in-wpf
            Typeface typeFace = new Typeface(slider.FontFamily, slider.FontStyle, slider.FontWeight, slider.FontStretch);
            double fontSize = slider.FontSize;
            Brush textBrush = slider.Foreground;
            FlowDirection flowDirection = slider.FlowDirection;

            size.Width -= ReservedSpace;
            double range = Maximum - Minimum;
            double logicalToPhysical = size.Width/range;
            Point startPoint = new Point(ReservedSpace*0.5, Placement == TickBarPlacement.Bottom ? size.Height : 0d);

            if (slider.MinimumWithoutTolerance > Minimum)
            {
                double minValueX = startPoint.X + (slider.MinimumWithoutTolerance - Minimum) * logicalToPhysical;
                FormattedText formattedTextMinValue = new FormattedText(slider.MinimumWithoutTolerance.ToString(CultureInfo.InvariantCulture), CultureInfo.CurrentUICulture, flowDirection, typeFace, fontSize, textBrush);
                dc.DrawText(formattedTextMinValue, new Point(minValueX - formattedTextMinValue.Width/2, startPoint.Y));
            }

            if (slider.MaximumWithoutTolerance < Maximum)
            {
                double maxValueX = startPoint.X + (slider.MaximumWithoutTolerance - Minimum) * logicalToPhysical;
                FormattedText formattedTextMaxValue = new FormattedText(slider.MaximumWithoutTolerance.ToString(CultureInfo.InvariantCulture), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, fontSize, textBrush);
                dc.DrawText(formattedTextMaxValue, new Point(maxValueX - formattedTextMaxValue.Width/2, startPoint.Y));
            }

            //System.Diagnostics.Debug.WriteLine(String.Format("==> {0} {1} |  {2}", minValueX, maxValueX, size.Width));
        }

        //private void DisplayTickAndValue(DrawingContext dc)
        //{
        //    Size size = new Size(ActualWidth, ActualHeight);

        //    double range = Maximum - Minimum;
        //    double tickLen = 0.0d; // Height for Primary Tick (for Mininum and Maximum value)
        //    double tickLen2; // Height for Secondary Tick
        //    double logicalToPhysical = 1.0;
        //    double progression = 1.0d;
        //    Point startPoint = new Point(0d, 0d);
        //    Point endPoint = new Point(0d, 0d);

        //    // Take Thumb size in to account
        //    double halfReservedSpace = ReservedSpace*0.5;

        //    switch (Placement)
        //    {
        //        case TickBarPlacement.Top:
        //            size.Width -= ReservedSpace;
        //            tickLen = -size.Height;
        //            startPoint = new Point(halfReservedSpace, size.Height);
        //            endPoint = new Point(halfReservedSpace + size.Width, size.Height);
        //            logicalToPhysical = size.Width/range;
        //            progression = 1;
        //            break;

        //        case TickBarPlacement.Bottom:
        //            size.Width -= ReservedSpace;
        //            tickLen = size.Height;
        //            startPoint = new Point(halfReservedSpace, 0d);
        //            endPoint = new Point(halfReservedSpace + size.Width, 0d);
        //            logicalToPhysical = size.Width/range;
        //            progression = 1;
        //            break;
        //    }

        //    tickLen2 = tickLen*0.75;

        //    Pen pen = new Pen(Fill, 1.0d);

        //    if (Placement == TickBarPlacement.Top || Placement == TickBarPlacement.Bottom)
        //    {
        //        double interval = TickFrequency;
        //        for (double i = interval; i < range; i += interval)
        //        {
        //            double x = i*logicalToPhysical + startPoint.X;
        //            dc.DrawLine(pen,
        //                        new Point(x, startPoint.Y),
        //                        new Point(x, startPoint.Y + tickLen2));

        //            double value = Minimum + i;
        //            FormattedText formattedText = new FormattedText(value.ToString(CultureInfo.InvariantCulture), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 12, Brushes.Black);
        //            dc.DrawText(formattedText, new Point(x - formattedText.Width/2, 10));

        //            if (Math.Abs(i - 20) < 0.001 || Math.Abs(i - 300) < 0.001)
        //                System.Diagnostics.Debug.WriteLine(String.Format("{0} | {1} -> {2}    {3}  {4}", i, value, x, halfReservedSpace, size.Width));
        //        }
        //    }
        //}

        protected override void OnRender(DrawingContext dc)
        {
            if (Placement != TickBarPlacement.Top && Placement != TickBarPlacement.Bottom)
                return;

            //DisplayTickAndValue(dc);
            DisplayMinAndMax(dc);
        }
    }
}
