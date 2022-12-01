using System.Collections.Generic;
using System.Numerics;

namespace BlazorGraphics.Shapes;

public class Cube : Shape
{
    private static readonly Vector3 planeNormal = new(0, 0, 1);

    public Cube(float x, float y, float z, float w, float h, float d)
    {
        var w2 = w / 2.0f;
        var h2 = h / 2.0f;
        var d2 = d / 2.0f;

        Nodes = new Vector3[]
        {
            new(x - w2, y - h2, z - d2),
            new(x - w2, y - h2, z + d2),
            new(x - w2, y + h2, z - d2),
            new(x - w2, y + h2, z + d2),
            new(x + w2, y - h2, z - d2),
            new(x + w2, y - h2, z + d2),
            new(x + w2, y + h2, z - d2),
            new(x + w2, y + h2, z + d2)
        };
    }

    public void Draw(Aptacode.BlazorCanvas.BlazorCanvas canvas, Vector3 A, Vector3 B, Vector3 C, Vector3 D, string Color)
    {
        if (Vector3.Dot(Vector3.Cross(B - A, B - C), planeNormal) < 0)
        {
            return;
        }

        canvas.Polygon(new[] {
                new Vector2(A.X, A.Y),
                new Vector2(B.X, B.Y),
                new Vector2(C.X, C.Y),
                new Vector2(D.X, D.Y),
                new Vector2(A.X, A.Y)
            });

        if (FacesOn)
        {
            canvas.FillStyle(Color);
            canvas.Fill();
        }

        if (EdgesOn)
        {
            canvas.StrokeStyle(EdgeColor);
            canvas.Stroke();
        }
    }

    public override void Draw(Aptacode.BlazorCanvas.BlazorCanvas x, float dt, Matrix4x4 transform)
    {
        x.LineWidth(Stroke);

        Draw(x, transform.Multiply(Nodes[3]), transform.Multiply(Nodes[1]), transform.Multiply(Nodes[5]), transform.Multiply(Nodes[7]), "#87230c");
        Draw(x, transform.Multiply(Nodes[2]), transform.Multiply(Nodes[3]), transform.Multiply(Nodes[7]), transform.Multiply(Nodes[6]), "#bcc21d");
        Draw(x, transform.Multiply(Nodes[0]), transform.Multiply(Nodes[2]), transform.Multiply(Nodes[6]), transform.Multiply(Nodes[4]), "#18a82b");
        Draw(x, transform.Multiply(Nodes[0]), transform.Multiply(Nodes[1]), transform.Multiply(Nodes[3]), transform.Multiply(Nodes[2]), "#181fa8");
        Draw(x, transform.Multiply(Nodes[5]), transform.Multiply(Nodes[4]), transform.Multiply(Nodes[6]), transform.Multiply(Nodes[7]), "#9218ab");
        Draw(x, transform.Multiply(Nodes[1]), transform.Multiply(Nodes[0]), transform.Multiply(Nodes[4]), transform.Multiply(Nodes[5]), "#a1123a");
    }
}