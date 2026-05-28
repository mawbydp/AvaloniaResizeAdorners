using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
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
        var polygon = new Polygon
        {
            Points = new Points
            {
                new Point(0, 0),
                new Point(100, 0),
                new Point(100, 50),
                new Point(0, 50)
            },

            Classes = { "Default" },
        };

        Canvas.SetLeft(polygon, 50);
        Canvas.SetTop(polygon, 50);

        canvas.Children.Add(polygon);

        AddAdorner(polygon, canvas);
    }

    private void AddAdorner(Polygon polygon, Canvas canvas)
    {
        AdornerLayer? layer = AdornerLayer.GetAdornerLayer(polygon);

        if (layer != null)
        {
            var selection = new PolygonAdorner(polygon, canvas);
            layer.Children.Add(selection);
        }
    }
}
