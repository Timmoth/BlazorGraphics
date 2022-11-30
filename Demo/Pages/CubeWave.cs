using System.Drawing;
using System.Numerics;
using Aptacode.BlazorCanvas;
using BlazorGraphics.Shapes;

namespace BlazorGraphics;

public class CubeWave
{
    private readonly List<Shape> Shapes = new();

    public CubeWave()
    {
        var cubeTranslateGroup = new Group
        {
            Matrix = Matrix4x4.Identity.Translate(new Vector3(250, 250, 0)),
        };

        var cubeGroup = new Group
        {
            Animator = new Animator()
            {
                DxRotation = (dt) => 0.01f * dt,
                DyRotation = (dt) => 0.015f * dt,
                DzRotation = (dt) => 0.02f * dt,
            }
        };

        var size = 30;
        var margin = 50;
        for (var x = -10; x < 10; x++)
        {
            for (var y = -10; y < 10; y++)
            {
                var pos = x + y;
                var cube = new Cube(0, 0, 0, size, size, size)
                {
                    Matrix = Matrix4x4.Identity.Translate(new Vector3(x * (size + margin), y * (size + margin), 1)),
                    Animator = new Animator()
                    {
                        DTranslate = (dt) => new Vector3(0, 0, (float)(Math.Sin((dt + pos) / 2) * 40)),
                    }
                };

                cubeGroup.Shapes.Add(cube);
            }
        }

        cubeTranslateGroup.Shapes.Add(cubeGroup);

        Shapes.Add(cubeTranslateGroup);
    }

    public string Background { get; set; } = "#4287f5";

    private int frame = 0;
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