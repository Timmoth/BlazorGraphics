using MauiGraphicsSandbox.Shapes;
using System.Numerics;

namespace MauiGraphicsSandbox
{
    public class GraphicsDrawable : IDrawable
    {
        public Color Background { get; set; } = Color.FromRgb(200,200,200);

        private readonly List<Shape> Shapes = new();

        private readonly Group group = new();
        public GraphicsDrawable()
        {
            group.TransformationData.Translation = new Vector3(500, 500, 1);

            group.Animator = new Animator(group.TransformationData)
            {
                DxRotation = 0.0005f,
                DyRotation = 0.0007f
            };
  
            var rand = new Random();
            for(int i = 0; i <50; i++)
            {
                var dist = 400;
                var cube = new Cube(rand.Next(-dist, dist), rand.Next(-dist, dist), rand.Next(-dist, dist), rand.Next(20, 50), rand.Next(20, 50), rand.Next(20, 50));

                cube.Animator = new Animator(cube.TransformationData)
                {
                    DxRotation = ((float)rand.NextDouble() - 0.5f) / 10f,
                    DyRotation = ((float)rand.NextDouble() - 0.5f) / 10f,
                    DzRotation = ((float)rand.NextDouble() - 0.5f) / 10f,
                };

                group.Shapes.Add(cube);
            }
            //group.Shapes.Add(new MyPath(new Vector3(-10, 20, 30), new Vector3(100, 150, 20), new Vector3(50, 43, 30)));
            Shapes.Add(group);
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            Clear(canvas, dirtyRect);
            var dt = 1;

            var orderedShapes = Shapes.Select(x => x.Draw(new Transform(), 1)).OrderByDescending(x => x.zIndex).ToList();
            for(int i = 0; i < orderedShapes.Count; i++)
            {
                orderedShapes[i].Item2.Invoke(canvas);
            }
        }

        private void Clear(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Background;
            canvas.FillRectangle(dirtyRect);
        }
    }

}
