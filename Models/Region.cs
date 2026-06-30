using Avalonia.Controls.Shapes;

namespace Aquarius.Models
{
    public class Region
    {
        public string Name { get; set; } = string.Empty;
        
        public Polygon Polygon { get; set; } = new();

        public double Doping { get; set; }
    }
}
