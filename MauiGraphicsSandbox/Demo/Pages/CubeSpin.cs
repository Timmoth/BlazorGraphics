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
        //group.TransformationData.Translation = new Vector3(500, 500, 1);

        //group.Animator = new Animator(group.TransformationData)
        //{
        //    DxRotation = 0.0005f,
        //    DyRotation = 0.0007f
        //};

        //var rand = new Random();
        //for (var i = 0; i < 100; i++)
        //{
        //    var dist = 400;
        //    var cube = new Cube(rand.Next(-dist, dist), rand.Next(-dist, dist), rand.Next(-dist, dist),
        //        rand.Next(20, 50), rand.Next(20, 50), rand.Next(20, 50));

        //    cube.Animator = new Animator(cube.TransformationData)
        //    {
        //        DxRotation = ((float)rand.NextDouble() - 0.5f) / 10f,
        //        DyRotation = ((float)rand.NextDouble() - 0.5f) / 10f,
        //        DzRotation = ((float)rand.NextDouble() - 0.5f) / 10f
        //    };

        //    group.Shapes.Add(cube);
        //}

        //Shapes.Add(group);
    }

    public void Add(Shape shape)
    {
        Shapes.Add(shape);
    }

    public string Background { get; set; } = "#4287f5";

    public void Draw(BlazorCanvas canvas, Rectangle dirtyRect)
    {
        Clear(canvas, dirtyRect);

        var orderedShapes = Shapes.Select(x => x.Draw(Matrix4x4.Identity, 1)).OrderByDescending(x => x.zIndex).ToList();
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