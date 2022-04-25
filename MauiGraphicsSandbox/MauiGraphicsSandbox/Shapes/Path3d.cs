using System.Numerics;

namespace MauiGraphicsSandbox.Shapes
{
    public class Path3d : Shape
    {
        public Path3d(params Vector3[] positions)
        {
            Nodes = positions.ToArray();
        }

        public override void Draw(ICanvas x, List<Vector3> nodes, float dt)
        {
            x.StrokeLineCap = LineCap.Round;
            x.StrokeSize = Stroke;
            x.StrokeColor = EdgeColor;

            var last = nodes[0];
            for (int i = 1; i < Nodes.Length; i++)
            {
                var node = nodes[i];
                x.DrawLine(last.X, last.Y, node.X, node.Y);
                last = node;
            }
        }
    }

}
