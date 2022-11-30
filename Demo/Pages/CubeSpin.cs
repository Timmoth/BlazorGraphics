using System.Drawing;
using System.Numerics;
using Aptacode.BlazorCanvas;
using BlazorGraphics.Shapes;

namespace BlazorGraphics;

public class CubeSpin
{
    private readonly Group group = new();

    private readonly List<Shape> Shapes = new();

    public CubeSpin()
    {
        var outer = new Group()
        {
            Matrix = Matrix4x4.Identity.Translate(new Vector3(250, 250, 1)),
        };
        group = new Group()
        {
            Animator = new Animator()
            {
                DxRotation = (dt) => 0.001f * dt,
                DyRotation = (dt) => 0.015f * dt,
                DzRotation = (dt) => 0.02f * dt,
            }
        };

        var rand = new Random();
        for (var i = 0; i < 1000; i++)
        {
            var xR = rand.NextDouble();
            var yR = rand.NextDouble();
            var zR = rand.NextDouble();

            var dist = 400;
            var cube = new Cube(rand.Next(-dist, dist), rand.Next(-dist, dist), rand.Next(-dist, dist),
                rand.Next(20, 50), rand.Next(20, 50), rand.Next(20, 50))
            {
                Animator = new Animator()
                {
                    DxRotation = (dt) => (float)(xR * 2 * Math.PI),
                    DyRotation = (dt) => (float)(yR * 2 * Math.PI),
                    DzRotation = (dt) => (float)(zR * 2 * Math.PI),
                }
            };

            group.Shapes.Add(cube);
        }

        outer.Shapes.Add(group);
        Shapes.Add(outer);
    }

    public void Add(Shape shape)
    {
        Shapes.Add(shape);
    }

    public string Background { get; set; } = "#4287f5";

    int frame = 0;
    public void Draw(BlazorCanvas canvas, Rectangle dirtyRect)
    {
        Clear(canvas, dirtyRect);
        frame++;
        var orderedShapes = Shapes.Select(x => x.Draw(Matrix4x4.Identity, frame)).OrderByDescending(x => x.zIndex).ToList();
        for (var i = 0; i < orderedShapes.Count; i++)
        {
            orderedShapes[i].Item2.Invoke(canvas);
        }
    }

    private void Clear(BlazorCanvas canvas, Rectangle dirtyRect)
    {
        canvas.ClearRect(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);
        canvas.FillStyle(Background);
        canvas.FillRect(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);
    }
}