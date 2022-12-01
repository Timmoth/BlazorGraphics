using System;
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

    public override void Draw(Aptacode.BlazorCanvas.BlazorCanvas canvas, float dt, Matrix4x4 transform)
    {
        var polyLineNodes = new Vector2[Nodes.Length];
        Span<Vector3> nodesAsSpan = Nodes;
        for (int i = 0; i < Nodes.Length; i++)
        {
            var node = transform.Multiply(nodesAsSpan[i]);
            polyLineNodes[i] = new Vector2(node.X, node.Y);
        }

        canvas.PolyLine(polyLineNodes);
        canvas.LineWidth(Stroke);
        canvas.StrokeStyle(EdgeColor);
        canvas.Stroke();
    }
}