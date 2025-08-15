using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;

namespace AvaloniaResizeAdorners.Adorners;

public class PolygonAdorner : Canvas
{
    private readonly Polygon _polygon;
    private readonly Thumb _thumb;

    public PolygonAdorner(Polygon adornedPolygon, Canvas canvas)
    {
        _polygon = adornedPolygon;
        
        _thumb = new Thumb
        {
            Width = 10,
            Height = 10,
            Classes = { "ResizeThumb" },
            Cursor = new Cursor(StandardCursorType.BottomRightCorner)
        };

        _thumb.DragDelta += Thumb_DragDelta;
        canvas.PointerPressed += _canvas_PointerPressed;
        _polygon.PointerPressed += _polygon_PointerPressed;

        UpdateThumbPosition();

        Children.Add(_thumb);
    }

    private void _polygon_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _polygon.Opacity = 0.5;
        _thumb.IsHitTestVisible = true;
    }

    private void _canvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_polygon != null && !_polygon.IsPointerOver)
        {
            _polygon.Opacity = 1.0;
            _thumb.IsHitTestVisible = false;
        }
    }

    private void Thumb_DragDelta(object? sender, Avalonia.Input.VectorEventArgs e)
    {
        if (_polygon.Points.Count != 4)
            return; // Assuming rectangle shape with 4 points.

        // Increase width and height based on drag
        double dx = e.Vector.X;
        double dy = e.Vector.Y;

        // Update polygon points
        _polygon.Points = new Points
        {
            new Point(0, 0),
            new Point(_polygon.Points[1].X + dx, 0),
            new Point(_polygon.Points[2].X + dx, _polygon.Points[2].Y + dy),
            new Point(0, _polygon.Points[3].Y + dy)
        };

        UpdateThumbPosition();
    }

    private void UpdateThumbPosition()
    {
        // Assuming top-left anchored polygon
        double left = Canvas.GetLeft(_polygon);
        double top = Canvas.GetTop(_polygon);

        double right = left + _polygon.Points[1].X;
        double bottom = top + _polygon.Points[2].Y;

        SetLeft(_thumb, right + _thumb.Width / 2);
        SetTop(_thumb, bottom + _thumb.Height / 2);
    }


}
