using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace AvaloniaResizeAdorners.Adorners;

public class PolygonAdorner : Canvas
{
    private readonly Polygon polygon;
    private readonly Thumb thumb;
    private bool isSelected;

    public PolygonAdorner(Polygon adornedPolygon, Canvas canvas)
    {
        polygon = adornedPolygon;
        
        thumb = new Thumb
        {
            Width = 10,
            Height = 10,
            Classes = { "ResizeThumb" },
            Cursor = new Cursor(StandardCursorType.BottomRightCorner)
        };

        thumb.DragDelta += Thumb_DragDelta;
        canvas.PointerPressed += Canvas_PointerPressed;
        polygon.PointerPressed += Polygon_PointerPressed;

        UpdateThumbPosition();

        Children.Add(thumb);
    }

    private void Polygon_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (isSelected && sender is Visual visual)
        {
            var point = e.GetCurrentPoint(visual);
            
            // Add context menu
            if (point.Properties.IsRightButtonPressed && sender is Control control)
            {
                var menu = new ContextMenu();

                MenuItem item1 = new MenuItem { Header = "Show Window..." };
                MenuItem item2 = new MenuItem { Header = "Change Colour..." };

                item1.Click += Item1_Click;
                item2.Click += Item2_Click; ;

                menu.Items.Add(item1);
                menu.Items.Add(item2);

                menu.Open(control);
            }
        }
        else
        {
            polygon.Opacity = 0.5;
            thumb.IsHitTestVisible = true;
            isSelected = true;
        }
    }

    private void Item1_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        new Window().Show();
    }

    private void Item2_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var rand = new Random();

        polygon.Fill = new SolidColorBrush(Color.FromRgb(
            (byte)rand.Next(256),
            (byte)rand.Next(256),
            (byte)rand.Next(256)
        ));
    }

    private void Canvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (polygon != null && !polygon.IsPointerOver)
        {
            polygon.Opacity = 1.0;
            thumb.IsHitTestVisible = false;
            isSelected = false;
        }
    }

    private void Thumb_DragDelta(object? sender, VectorEventArgs e)
    {
        // Assuming rectangle shape with 4 points.
        if (polygon.Points.Count != 4)
            return;

        // Increase width and height based on drag
        double dx = e.Vector.X;
        double dy = e.Vector.Y;

        // Update polygon points
        polygon.Points = new Points
        {
            new Point(0, 0),
            new Point(polygon.Points[1].X + dx, 0),
            new Point(polygon.Points[2].X + dx, polygon.Points[2].Y + dy),
            new Point(0, polygon.Points[3].Y + dy)
        };

        UpdateThumbPosition();
    }

    private void UpdateThumbPosition()
    {
        // Assuming top-left anchored polygon
        double left = Canvas.GetLeft(polygon);
        double top = Canvas.GetTop(polygon);

        double right = left + polygon.Points[1].X;
        double bottom = top + polygon.Points[2].Y;

        SetLeft(thumb, right + thumb.Width / 2);
        SetTop(thumb, bottom + thumb.Height / 2);
    }
}
