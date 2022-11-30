using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace BlazorGraphics.Shapes;
public class Path3d : Shape
{
    public Path3d(params Vector3[] positions)
    {
        Nodes = positions.ToArray();
    }

    public override void Draw(Aptacode.BlazorCanvas.BlazorCanvas x, Vector3[] nodes, float dt, Matrix4x4 transform)
    {
        x.LineWidth(Stroke);
        x.StrokeStyle(EdgeColor);

        var first = nodes[0];
        x.MoveTo(first.X, first.Y);
        for (var i = 1; i < Nodes.Length; i++)
        {
            var node = nodes[i];
            x.LineTo(node.X, node.Y);
        }
    }
}