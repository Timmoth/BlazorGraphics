using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace BlazorGraphics.Shapes;
public class Path3d : Shape
{
    public Path3d(params Vector3[] positions)
    {
        Nodes = positions.ToArray();
    }

    public override void Draw(Aptacode.BlazorCanvas.BlazorCanvas canvas, Vector3[] nodes, float dt, Matrix4x4 transform)
    {
        canvas.PolyLine(nodes.Select(n => new Vector2(n.X, n.Y)).ToArray());
        canvas.LineWidth(Stroke);
        canvas.StrokeStyle(EdgeColor);
        canvas.Stroke();
    }
}