using System.Numerics;

namespace MauiGraphicsSandbox.Shapes
{
    public abstract class Shape
    {
        protected Vector3[] Nodes { get; init; } = Array.Empty<Vector3>();
        public Color NodeColor { get; set; } = Color.FromRgb(10, 10, 10);
        public Color EdgeColor { get; set; } = Color.FromRgb(10, 10, 10);
        public bool FacesOn { get; set; } = true;
        public bool EdgesOn { get; set; } = true;
        public bool NodesOn { get; set; } = true;
        public int Stroke { get; set; } = 20;

        public Transform TransformationData { get; } = new();
        public Animator Animator;
        public virtual (float zIndex, Action<ICanvas>) Draw(Transform transform, float dt)
        {
            Animator?.Apply(dt);
            var matrix = Transform.Combine(transform, TransformationData).CreateTransformationMatrix();
            var nodes = Nodes.Select(x => Transform.Multiply(matrix, x)).ToList();
            var zIndex = nodes.Count == 0 ? 0 : nodes.Select(x => x.Z).Min();
            return (zIndex, x => Draw(x, nodes, dt));
        }

        public abstract void Draw(ICanvas x, List<Vector3> nodes, float dt);
    }

}
