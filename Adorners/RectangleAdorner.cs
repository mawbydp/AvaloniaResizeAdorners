namespace AvaloniaResizeAdorners.Adorners;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using SkiaSharp;
using System;
using System.Collections.Generic;

public class RectangleAdorner : Canvas
{
    private Polygon _polygon;
    private List<Thumb> _thumbs = new();

    public RectangleAdorner(Rectangle rectangle)
    {
        //_polygon = adornedPolygon;
        //SetRandomFill();
        //CreateThumb();

        var rect = (new Rectangle
        {
            Width = 100,
            Height = 50,
            Classes = { "Deafult" },
        });


        Canvas.SetLeft(rect, 50);
        Canvas.SetTop(rect, 50);

        Children.Add(rect);
    }

    private void SetRandomFill()
    {
        var random = new Random();
        var color = Color.FromArgb(
            255,  // Full opacity
            (byte)random.Next(256),
            (byte)random.Next(256),
            (byte)random.Next(256)
        );

        this.Background = new SolidColorBrush(color);
    }


    private void CreateThumb()
    {
        if (_polygon.Points.Count == 0) return;

        var centroid = GetCentroid(_polygon.Points);

        /*var thumb = new Thumb
        {
            Width = 100,
            Height = 100,
            Background = Brushes.Red
        };

        _thumbs.Add(thumb);
        Children.Add(thumb);

        

        // Explicitly position it within the Canvas
        Canvas.SetLeft(thumb, centroid.X - 5);
        Canvas.SetTop(thumb, centroid.Y - 5);*/

        //var centroid = GetCentroid(_polygon.Points);

        var random = new Random();
        var color = Color.FromArgb(
            255,  // Full opacity
            (byte)random.Next(256),
            (byte)random.Next(256),
            (byte)random.Next(256)
        );

        var circle = new Ellipse
        {
            Width = 10,
            Height = 10,
            //Fill = Brushes.Red,
            Fill = new SolidColorBrush(color),
            Stroke = Brushes.Black,
            StrokeThickness = 1
        };

        Children.Add(circle);

        Canvas.SetLeft(circle, centroid.X - circle.Width / 2);
        Canvas.SetTop(circle, centroid.Y - circle.Height / 2);

        //Canvas.SetZIndex(thumb, 1000); // Ensure the thumb is above the polygon
    }


    private void ArrangePointThumb(Thumb thumb, Point point, double thumbSize)
    {
        double offset = thumbSize / 2;

        if (!double.IsNaN(offset))
        {
            thumb.Arrange(new Rect(point.X - offset, point.Y - offset, thumbSize, thumbSize));
        }
    }

    // Function to calculate the centroid of a polygon
    private Point GetCentroid(IList<Point> points)
    {
        double xSum = 0, ySum = 0;
        int count = points.Count;

        foreach (var point in points)
        {
            xSum += point.X;
            ySum += point.Y;
        }

        return new Point(xSum / count, ySum / count);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return base.ArrangeOverride(finalSize);
    }
}

