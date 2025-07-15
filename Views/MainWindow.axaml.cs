using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using AvaloniaResizeAdorners.Adorners;

namespace AvaloniaResizeAdorners.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += MainWindow_Opened;
    }

    private void MainWindow_Opened(object? sender, System.EventArgs e)
    {
        var rect = (new Polygon
        {
            Points = new Points
            {
                new Point(0, 0),
                new Point(100, 0),
                new Point(100, 50),
                new Point(0, 50)
            },

            Classes = { "Default" },
        });

        Canvas.SetLeft(rect, 50);
        Canvas.SetTop(rect, 50);

        myCanvas.Children.Add(rect);

        rect.PointerPressed += Rect_PointerPressed;
    }

    private static void Rect_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Polygon polygon)
        {
            AdornerLayer? layer = AdornerLayer.GetAdornerLayer(polygon);

            if (layer != null)
            {
                var selection = new PolygonAdorner(polygon);
                layer.Children.Add(selection);
            }
        }
    }
}