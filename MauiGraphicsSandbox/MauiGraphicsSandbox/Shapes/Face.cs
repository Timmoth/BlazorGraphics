using System.Numerics;

namespace MauiGraphicsSandbox.Shapes
{
    public record struct Face(int A, int B, int C, int D, Color Color)
    {
        public FaceNodes GetNodes(List<Vector3> Nodes)
        {
            return new(Nodes[A], Nodes[B], Nodes[C], Nodes[D], Color);
        }
    }
}
