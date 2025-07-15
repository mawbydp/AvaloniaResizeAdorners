using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;

namespace AvaloniaResizeAdorners.Adorners;

public class PolygonAdorner : Canvas
{
    private readonly Polygon _adornedPolygon;
    private readonly Thumb _thumb;

    public PolygonAdorner(Polygon adornedPolygon)
    {
        _adornedPolygon = adornedPolygon;

        _thumb = new Thumb
        {
            Width = 10,
            Height = 10,
            Classes = { "ResizeThumb" },
            Cursor = new Cursor(StandardCursorType.BottomRightCorner)
        };

        _thumb.DragDelta += Thumb_DragDelta;

        UpdateThumbPosition();

        Children.Add(_thumb);
    }

    private void Thumb_DragDelta(object? sender, Avalonia.Input.VectorEventArgs e)
    {
        if (_adornedPolygon.Points.Count != 4)
            return; // Assuming rectangle shape with 4 points.

        // Increase width and height based on drag
        double dx = e.Vector.X;
        double dy = e.Vector.Y;

        // Update polygon points
        _adornedPolygon.Points = new Points
        {
            new Point(0, 0),
            new Point(_adornedPolygon.Points[1].X + dx, 0),
            new Point(_adornedPolygon.Points[2].X + dx, _adornedPolygon.Points[2].Y + dy),
            new Point(0, _adornedPolygon.Points[3].Y + dy)
        };

        UpdateThumbPosition();
    }

    private void UpdateThumbPosition()
    {
        // Assuming top-left anchored polygon
        double left = Canvas.GetLeft(_adornedPolygon);
        double top = Canvas.GetTop(_adornedPolygon);

        double right = left + _adornedPolygon.Points[1].X;
        double bottom = top + _adornedPolygon.Points[2].Y;

        SetLeft(_thumb, right + _thumb.Width / 2);
        SetTop(_thumb, bottom + _thumb.Height / 2);
    }
}
