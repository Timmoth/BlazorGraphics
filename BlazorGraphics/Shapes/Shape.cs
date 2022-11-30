using System;
using System.Numerics;

namespace BlazorGraphics.Shapes;

public abstract class Shape
{
    public Animator Animator { get; set; } = new();
    protected Vector3[] Nodes { get; init; } = Array.Empty<Vector3>();
    public string NodeColor { get; set; } = "black";
    public string EdgeColor { get; set; } = "black";
    public bool FacesOn { get; set; } = true;
    public bool EdgesOn { get; set; } = true;
    public bool NodesOn { get; set; } = true;
    public int Stroke { get; set; } = 2;

    public Matrix4x4 Matrix { get; set; } = Matrix4x4.Identity;

    public virtual (float zIndex, Action<Aptacode.BlazorCanvas.BlazorCanvas>) Draw(Matrix4x4 transform, float dt)
    {
        var matrix = Matrix4x4.Multiply(Animator.Apply(dt, Matrix), transform);

        var nodes = new Vector3[Nodes.Length];

        var zIndex = 1000f;
        Span<Vector3> nodesAsSpan = Nodes;
        for(int i = 0; i < nodes.Length; i++)
        {
            var node = matrix.Multiply(nodesAsSpan[i]);
            if(node.Z < zIndex)
            {
                zIndex = node.Z;
            }

            nodes[i] = node;
        }

        return (zIndex, x => Draw(x, nodes, dt, matrix));
    }

    public abstract void Draw(Aptacode.BlazorCanvas.BlazorCanvas x, Vector3[] nodes, float dt, Matrix4x4 transform);
}