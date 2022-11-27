using System.Numerics;

namespace BlazorGraphics.Shapes;

public record struct Face(int A, int B, int C, int D, string Color)
{
    public FaceNodes GetNodes(List<Vector3> Nodes)
    {
        return new FaceNodes(Nodes[A], Nodes[B], Nodes[C], Nodes[D], Color);
    }
}