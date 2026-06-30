using Aquarius.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using MsBox.Avalonia;
using Avalonia.Interactivity;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;

namespace Aquarius.Adorners;

public class PolygonAdorner : Canvas
{
    private readonly Polygon polygon;
    private readonly List<Thumb> pointThumbs = [];
    private bool isSelected;
    private const double thumbSize = 10;
    private bool isDragging;
    private Point lastPointerPosition;

    public PolygonAdorner(Region adornedRegion, Canvas canvas)
    {
        polygon = adornedRegion.Polygon;

        polygon.PointerPressed += Polygon_PointerPressed;
        polygon.PointerReleased += Polygon_PointerReleased;
        polygon.PointerMoved += Polygon_PointerMoved;
        canvas.PointerPressed += Canvas_PointerPressed;

        Loaded += PolygonAdorner_Loaded;
    }

    private void PolygonAdorner_Loaded(object? sender, RoutedEventArgs e)
    {
        AddThumbs();
    }

    private void Polygon_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!isDragging)
        {
            return;
        }

        Point currentPos = e.GetPosition(polygon);

        double dx = currentPos.X - lastPointerPosition.X;
        double dy = currentPos.Y - lastPointerPosition.Y;

        MovePolygon(dx, dy);

        lastPointerPosition = currentPos;
    }

    private void MovePolygon(double dx, double dy)
    {
        var pts = polygon.Points;

        for (int i = 0; i < pts.Count; i++)
        {
            pts[i] = new Point(
                pts[i].X + dx,
                pts[i].Y + dy
            );
        }

        polygon.Points = [.. pts];

        UpdateThumbPositions();
    }

    private void Polygon_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        isDragging = false;
        polygon.Cursor = new Cursor(StandardCursorType.Arrow);
    }

    private void AddThumbs()
    {
        Children.Clear();
        pointThumbs.Clear();

        for (int i = 0; i < polygon.Points.Count; i++)
        {
            int index = i;

            var thumb = new Thumb
            {
                Width = thumbSize,
                Height = thumbSize,
                Classes = { "ResizeThumb" },
                Cursor = new Cursor(StandardCursorType.BottomRightCorner),
                IsHitTestVisible = false
            };

            thumb.DragDelta += (s, e) => DragPoint(index, e.Vector);

            pointThumbs.Add(thumb);
            Children.Add(thumb);
        }

        UpdateThumbPositions();
    }

    private void DragPoint(int index, Vector delta)
    {
        var points = polygon.Points;

        points[index] = new Point(
            points[index].X + delta.X,
            points[index].Y + delta.Y
        );

        polygon.Points = [.. points];

        UpdateThumbPositions();
    }

    private void UpdateThumbPositions()
    {
        for (int i = 0; i < pointThumbs.Count; i++)
        {
            var p = polygon.Points[i];

            var transformed = polygon.TranslatePoint(p, this);

            if (transformed.HasValue)
            {
                SetLeft(pointThumbs[i], transformed.Value.X - thumbSize / 2);
                SetTop(pointThumbs[i], transformed.Value.Y - thumbSize / 2);
            }
        }
    }

    private void Polygon_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            ShowWindow();
            return;
        }

        if (!isSelected)
        {
            polygon.Opacity = 0.5;
            isSelected = true;

            foreach (var thumb in pointThumbs)
            {
                thumb.IsHitTestVisible = true;
            }
        }
        else if (e.GetCurrentPoint(polygon).Properties.IsLeftButtonPressed)
        {
            isDragging = true;
            lastPointerPosition = e.GetPosition(polygon);
            polygon.Cursor = new Cursor(StandardCursorType.SizeAll);
        }
        else
        {
            ShowContextMenu(sender, e);
        }
    }

    private void ShowContextMenu(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Visual visual)
        {
            return;
        }

        PointerPoint point = e.GetCurrentPoint(visual);

        if (point.Properties.IsRightButtonPressed && sender is Control control)
        {
            var menu = new ContextMenu();

            MenuItem item1 = new() { Header = "Show Window..." };
            MenuItem item2 = new() { Header = "Change Colour..." };

            item1.Click += Item1_Click;
            item2.Click += Item2_Click;

            menu.Items.Add(item1);
            menu.Items.Add(item2);

            menu.Open(control);
        }
    }

    private static void Item1_Click(object? sender, RoutedEventArgs e)
    {
        ShowWindow();
    }

    private void Item2_Click(object? sender, RoutedEventArgs e)
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
        if (!polygon.IsPointerOver)
        {
            polygon.Opacity = 1.0;
            isSelected = false;

            foreach (var thumb in pointThumbs)
            {
                thumb.IsHitTestVisible = false;
            }
        }
    }

    private static void ShowWindow()
    {
        var messageBox = MessageBoxManager
                .GetMessageBoxStandard("Aquarius", "Hello! This mimics a property window!", ButtonEnum.OkCancel);

        messageBox.ShowAsync();
    }
}
