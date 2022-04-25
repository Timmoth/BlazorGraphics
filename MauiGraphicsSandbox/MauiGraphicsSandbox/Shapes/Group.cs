using System.Numerics;

namespace MauiGraphicsSandbox.Shapes
{
    public class Group : Shape
    {
        public List<Shape> Shapes { get; set; } = new List<Shape>();
        public override void Draw(ICanvas x, List<Vector3> nodes, float dt)
        {
            var orderedShapes = Shapes.Select(x => x.Draw(TransformationData, dt)).OrderByDescending(x => x.zIndex).ToList();
            for (int i = 0; i < orderedShapes.Count; i++)
            {
                orderedShapes[i].Item2.Invoke(x);
            }
        }
    }
}
