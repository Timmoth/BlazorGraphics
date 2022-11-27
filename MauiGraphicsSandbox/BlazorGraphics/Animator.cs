using System.Numerics;

namespace BlazorGraphics;

public class Animator
{
    public float? DxRotation { get; set; } = 0;
    public float? DyRotation { get; set; } = 0;
    public float? DzRotation { get; set; } = 0;

    public Func<float, Vector3>? DTranslate { get; set; }

    public Matrix4x4 Apply(float dt, Matrix4x4 matrix)
    {
        var translation = matrix;
        if (DxRotation.HasValue)
        {
            translation = translation.RotateX(DxRotation.Value * dt);
        }

        if (DyRotation.HasValue)
        {
            translation = translation.RotateY(DyRotation.Value * dt);
        }

        if (DzRotation.HasValue)
        {
            translation = translation.RotateZ(DzRotation.Value * dt);
        }

        if (DTranslate != null)
        {
            var rr = DTranslate.Invoke(dt);
            translation = translation.Translate(rr);
        }

        return translation;
    }
}