using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaResizeAdorners.Adorners;

public abstract class AdornerBase : Control
{
    private readonly Control _adornedElement;

    protected AdornerBase(Control adornedElement)
    {
        _adornedElement = adornedElement;
    }

    public Control AdornedElement => _adornedElement;

    protected void AddVisual(Visual visual)
    {
        VisualChildren.Add(visual);
        InvalidateVisual();
    }

    protected void RemoveVisual(Visual visual)
    {
        VisualChildren.Remove(visual);
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        foreach (var visual in VisualChildren)
        {
            if (visual is Control control)
            {
                control.Render(context);
            }
        }
    }
}
