using System;
using System.Linq;
using System.Numerics;

namespace BlazorGraphics.Shapes;
public class Path3d : Shape
{
    public Path3d(params Vector3[] positions)
    {
        Nodes = positions.ToArray();
    }

    public override void Draw(Aptacode.BlazorCanvas.BlazorCanvas canvas, float dt, Matrix4x4 transform)
    {
        var polyLineNodes = new double[Nodes.Length * 2];
        Span<Vector3> nodesAsSpan = Nodes;

        int index = 0;
        for (int i = 0; i < Nodes.Length; i++)
        {
            var node = transform.Multiply(nodesAsSpan[i]);
            polyLineNodes[index++] = node.X;
            polyLineNodes[index++] = node.Y;
        }

        canvas.PolyLine(polyLineNodes);
        canvas.LineWidth(Stroke);
        canvas.StrokeStyle(EdgeColor);
        canvas.Stroke();
    }
}