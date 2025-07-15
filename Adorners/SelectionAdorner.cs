using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaResizeAdorners.Adorners;

public class SelectionAdorner : AdornerBase
{
    public SelectionAdorner(Control adornedElement) 
        : base(adornedElement)
    {
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = new Rect(AdornedElement.Bounds.Size);
        var pen = new Pen(Brushes.Red, 2, new DashStyle(new double[] { 4, 4 }, 0));

        context.DrawRectangle(null, pen, bounds);
    }
}
