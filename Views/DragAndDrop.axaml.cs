using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Aquarius.Adorners;
using Aquarius.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Linq;

namespace Aquarius.Views;

public partial class DragAndDrop : Window
{
    private bool isDragging;
    private Point lastPointerPosition;

    public DragAndDrop()
    {
        InitializeComponent();

        // Register drop event handlers for the Canvas using the DragDrop static helper methods
        DragDrop.AddDragOverHandler(DropCanvas, Canvas_DragOver);
        DragDrop.AddDropHandler(DropCanvas, Canvas_Drop);
    }

    // 1. Kick off the Drag operation using Avalonia 12 DataTransfer
    private async void ToolbarItem_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && !string.IsNullOrEmpty(border.Name))
        {
            var dragData = new DataTransfer();

            // CreateText takes exactly 1 argument (the text itself)
            dragData.Add(DataTransferItem.CreateText(border.Name));

            // Execute the drag operation asynchronously
            await DragDrop.DoDragDropAsync(e, dragData, DragDropEffects.Copy);
        }
    }

    // 2. Validate that the canvas can accept the incoming data type
    private void Canvas_DragOver(object? sender, DragEventArgs e)
    {
        // Look inside Formats using DataFormat.Text
        if (e.DataTransfer.Formats.Contains(DataFormat.Text))
        {
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    // 3. Handle the drop event to instantiate the control onto the canvas coordinates
    private void Canvas_Drop(object? sender, DragEventArgs e)
    {
        // Avalonia 12 fix: Look inside formats, then pull text synchronously using TryGetText()
        if (!e.DataTransfer.Formats.Contains(DataFormat.Text)) return;

        var itemType = e.DataTransfer.TryGetText();
        if (string.IsNullOrEmpty(itemType)) return;

        // Get the drop coordinates relative to the Canvas
        Point dropPosition = e.GetPosition(DropCanvas);

        Control? newVisualElement = null;

        // Factory pattern based on what was dragged from the toolbar
        if (itemType == "Resistor")
        {
            newVisualElement = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = Brushes.DeepSkyBlue,
                RadiusX = 4,
                RadiusY = 4
            };
        }
        else if (itemType == "Device")
        {
            newVisualElement = new Polygon
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
        }

        if (newVisualElement != null)
        {
            // Position the item dynamically centered on the mouse release coordinates
            Canvas.SetLeft(newVisualElement, dropPosition.X - (50 / 2.0));
            Canvas.SetTop(newVisualElement, dropPosition.Y - (50 / 2.0));

            // Add the item to the Canvas child collection
            DropCanvas.Children.Add(newVisualElement);

            if (newVisualElement is Polygon polygon)
            {
                var region = new Region
                {
                    Polygon = polygon,
                };

                AddAdorner(region, DropCanvas);
            }
            else
            {
                // Add dragging
                newVisualElement.PointerPressed += NewVisualElement_PointerPressed;
                newVisualElement.PointerMoved += NewVisualElement_PointerMoved;
                newVisualElement.PointerReleased += NewVisualElement_PointerReleased;
            }
        }
    }

    private void NewVisualElement_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Shape shape)
        {
            shape.Cursor = new Cursor(StandardCursorType.Arrow);
            isDragging = false;
        }
    }

    private void NewVisualElement_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!isDragging)
            return;

        if (sender is Shape shape)
        {
            shape.Cursor = new Cursor(StandardCursorType.SizeAll);

            // Get the current position relative to the Canvas
            var currentPos = e.GetPosition(DropCanvas);

            // Calculate the distance moved since last frame
            double dx = currentPos.X - lastPointerPosition.X;
            double dy = currentPos.Y - lastPointerPosition.Y;

            // Get the element's current positions (defaulting to 0 if not set)
            double currentLeft = Canvas.GetLeft(shape);
            
            if (double.IsNaN(currentLeft)) 
                currentLeft = 0;

            double currentTop = Canvas.GetTop(shape);
            if (double.IsNaN(currentTop)) 
                currentTop = 0;

            // Apply the offset changes
            Canvas.SetLeft(shape, currentLeft + dx);
            Canvas.SetTop(shape, currentTop + dy); // Fixed: SetTop instead of duplicated SetLeft

            // Update the baseline position for the next movement calculation loop
            lastPointerPosition = currentPos;
        }
    }

    private void NewVisualElement_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            var messageBox = MessageBoxManager
                .GetMessageBoxStandard("Aquarius", "Hello! This mimics a property window!", ButtonEnum.OkCancel);

            messageBox.ShowAsync();
            return;
        }

        if (sender is Shape shape)
        {
            shape.Cursor = new Cursor(StandardCursorType.Arrow);
            isDragging = true;
            lastPointerPosition = e.GetPosition(DropCanvas);
        }
    }

    private static void AddAdorner(Region region, Canvas canvas)
    {
        AdornerLayer? layer = AdornerLayer.GetAdornerLayer(region.Polygon);

        if (layer != null)
        {
            var selection = new PolygonAdorner(region, canvas);
            layer.Children.Add(selection);
        }
    }
}