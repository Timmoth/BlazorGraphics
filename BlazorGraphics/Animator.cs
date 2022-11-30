using System;
using System.Numerics;

namespace BlazorGraphics;

public class Animator
{
    public Func<float, float> DxRotation { get; set; }
    public Func<float, float> DyRotation { get; set; }
    public Func<float, float> DzRotation { get; set; }
    public Func<float, Vector3> DTranslate { get; set; }

    public Matrix4x4 Apply(float dt, Matrix4x4 matrix)
    {
        var translation = matrix;
        if (DxRotation != null)
        {
            translation = translation.RotateX(DxRotation.Invoke(dt));
        }

        if (DyRotation !=  null)
        {
            translation = translation.RotateY(DyRotation.Invoke(dt));
        }

        if (DzRotation != null)
        {
            translation = translation.RotateZ(DzRotation.Invoke(dt));
        }

        if (DTranslate != null)
        {
            translation = translation.Translate(DTranslate.Invoke(dt));
        }

        return translation;
    }
}