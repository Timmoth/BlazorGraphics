using System.Collections.Generic;
using System.Linq;
using System.Numerics;
namespace BlazorGraphics.Shapes;

public class Group : Shape
{
    public List<Shape> Shapes { get; set; } = new();

    public override void Draw(Aptacode.BlazorCanvas.BlazorCanvas x, float dt, Matrix4x4 transform)
    {
        var matrix = Matrix4x4.Multiply(Matrix, transform);

        var orderedShapes = Shapes.Select(s => s.Draw(matrix, dt)).OrderByDescending(s => s.zIndex)
            .ToList();
        for (var i = 0; i < orderedShapes.Count; i++)
        {
            orderedShapes[i].Item2.Invoke(x);
        }
    }
}