using System.Numerics;

namespace BlazorGraphics.Shapes;

public class Cube : Shape
{
    private static readonly Vector3 planeNormal = new(0, 0, 1);
    private Face _back = new(3, 1, 5, 7, "#61b814");
    private Face _bottom = new(2, 3, 7, 6, "#aded74");
    private Face _front = new(0, 2, 6, 4, "#85e332");
    private Face _left = new(0, 1, 3, 2, "#b87114");
    private Face _right = new(5, 4, 6, 7, "#b84014");
    private Face _top = new(1, 0, 4, 5, "#e4ffcc");

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

    public void Draw(Aptacode.BlazorCanvas.BlazorCanvas canvas, FaceNodes f)
    {
        if (Vector3.Dot(Vector3.Cross(f.B - f.A, f.B - f.C), planeNormal) < 0)
        {
            return;
        }

        if (FacesOn)
        {
            canvas.FillStyle(f.Color);
            canvas.BeginPath();
            canvas.MoveTo(f.A.X, f.A.Y);
            canvas.LineTo(f.B.X, f.B.Y);
            canvas.LineTo(f.C.X, f.C.Y);
            canvas.LineTo(f.D.X, f.D.Y);
            canvas.Fill();
            canvas.ClosePath();
        }

        if (EdgesOn)
        {
            canvas.StrokeStyle(EdgeColor);
            canvas.BeginPath();
            canvas.MoveTo(f.A.X, f.A.Y);
            canvas.LineTo(f.B.X, f.B.Y);
            canvas.LineTo(f.C.X, f.C.Y);
            canvas.LineTo(f.D.X, f.D.Y);
            canvas.LineTo(f.A.X, f.A.Y);
            canvas.Stroke();
            canvas.ClosePath();
        }
    }

    public override void Draw(Aptacode.BlazorCanvas.BlazorCanvas x, List<Vector3> nodes, float dt, Matrix4x4 transform)
    {
        x.LineWidth(Stroke);

        Draw(x, _front.GetNodes(nodes));
        Draw(x, _back.GetNodes(nodes));
        Draw(x, _left.GetNodes(nodes));
        Draw(x, _right.GetNodes(nodes));
        Draw(x, _top.GetNodes(nodes));
        Draw(x, _bottom.GetNodes(nodes));
    }
}