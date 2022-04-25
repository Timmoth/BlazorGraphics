using System.Numerics;

namespace MauiGraphicsSandbox.Shapes
{
    public class Cube : Shape
    {
        private Face _top = new(1, 0, 4, 5, Color.FromRgb(255, 0, 0));
        private Face _bottom = new(2, 3, 7, 6, Color.FromRgb(100, 0, 0));
        private Face _front = new(0, 2, 6, 4, Color.FromRgb(0, 255, 0));
        private Face _back = new(3, 1, 5, 7, Color.FromRgb(0, 100, 0));
        private Face _left = new(0, 1, 3, 2, Color.FromRgb(0, 0, 255));
        private Face _right = new(5, 4, 6, 7, Color.FromRgb(0, 0, 100));

        public Cube(float x, float y, float z, float w, float h, float d) : base()
        {
            var w2 = w / 2.0f;
            var h2 = h / 2.0f;
            var d2 = d / 2.0f;

            Nodes = new Vector3[]
            {
                 new (x - w2, y - h2, z - d2),
                 new (x - w2, y - h2, z + d2),
                 new (x - w2, y + h2, z - d2),
                 new (x - w2, y + h2, z + d2),
                 new (x + w2, y - h2, z - d2),
                 new (x + w2, y - h2, z + d2),
                 new (x + w2, y + h2, z - d2),
                 new (x + w2, y + h2, z + d2)
            };
        }

        private static readonly Vector3 planeNormal = new (0, 0, 1);

        public void Draw(ICanvas canvas, FaceNodes f)
        {
            if (Vector3.Dot(Vector3.Cross(f.B - f.A, f.B - f.C), planeNormal) < 0)
            {
                return;
            }

            if (FacesOn)
            {
                canvas.FillColor = f.Color;
                PathF path = new();
                path.MoveTo(f.A.X, f.A.Y);
                path.LineTo(f.B.X, f.B.Y);
                path.LineTo(f.C.X, f.C.Y);
                path.LineTo(f.D.X, f.D.Y);
                path.Close();
                canvas.FillPath(path);
            }

            if (EdgesOn)
            {
                canvas.StrokeColor = EdgeColor;
                canvas.DrawLine(f.A.X, f.A.Y, f.B.X, f.B.Y);
                canvas.DrawLine(f.B.X, f.B.Y, f.C.X, f.C.Y);
                canvas.DrawLine(f.C.X, f.C.Y, f.D.X, f.D.Y);
                canvas.DrawLine(f.D.X, f.D.Y, f.A.X, f.A.Y);
            }

            if (NodesOn)
            {
                canvas.FillColor = NodeColor;
                canvas.StrokeColor = NodeColor;
                canvas.DrawEllipse(f.A.X - Stroke / 2, f.A.Y - Stroke / 2, Stroke, Stroke);
                canvas.DrawEllipse(f.B.X - Stroke / 2, f.B.Y - Stroke / 2, Stroke, Stroke);
                canvas.DrawEllipse(f.C.X - Stroke / 2, f.C.Y - Stroke / 2, Stroke, Stroke);
                canvas.DrawEllipse(f.D.X - Stroke / 2, f.D.Y - Stroke / 2, Stroke, Stroke);
            }
        }

        public override void Draw(ICanvas x, List<Vector3> nodes, float dt)
        {
            x.StrokeLineCap = LineCap.Round;
            x.StrokeSize = Stroke;

            Draw(x, _front.GetNodes(nodes));
            Draw(x, _back.GetNodes(nodes));
            Draw(x, _left.GetNodes(nodes));
            Draw(x, _right.GetNodes(nodes));
            Draw(x, _top.GetNodes(nodes));
            Draw(x, _bottom.GetNodes(nodes));
        }
    }
}
