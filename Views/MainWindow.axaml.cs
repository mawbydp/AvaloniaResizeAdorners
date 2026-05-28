using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using AvaloniaResizeAdorners.Adorners;
using AvaloniaResizeAdorners.Models;

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
        Region region = new()
        {
            Name = "Test Name",
            Doping = 100,

            Polygon = new Polygon
            {
                Points = new Points
                {
                    new Point(0, 0),
                    new Point(100, 0),
                    new Point(100, 50),
                    new Point(0, 50)
                },

                Classes = { "Default" },
            }
        };

        Canvas.SetLeft(region.Polygon, 50);
        Canvas.SetTop(region.Polygon, 50);

        canvas.Children.Add(region.Polygon);

        AddAdorner(region, canvas);
    }

    private void AddAdorner(Region region, Canvas canvas)
    {
        AdornerLayer? layer = AdornerLayer.GetAdornerLayer(region.Polygon);

        if (layer != null)
        {
            var selection = new PolygonAdorner(region, canvas);
            layer.Children.Add(selection);
        }
    }
}
