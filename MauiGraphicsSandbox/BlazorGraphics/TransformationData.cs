using System.Numerics;

namespace BlazorGraphics;


public static class MatrixExtensions
{
    public static Matrix4x4 Scale(this Matrix4x4 matrix, Vector3 scale)
    {
        return Matrix4x4.Multiply(matrix, Matrix4x4.CreateScale(scale));
    }

    public static Matrix4x4 Translate(this Matrix4x4 matrix, Vector3 translation)
    {
        return Matrix4x4.Multiply(matrix, Matrix4x4.CreateTranslation(translation));
    }

    public static Matrix4x4 RotateX(this Matrix4x4 matrix, float xRotation)
    {
        return Matrix4x4.Multiply(matrix, Matrix4x4.CreateRotationX(xRotation));
    }

    public static Matrix4x4 RotateY(this Matrix4x4 matrix, float yRotation)
    {
        return Matrix4x4.Multiply(matrix, Matrix4x4.CreateRotationY(yRotation));
    }

    public static Matrix4x4 RotateZ(this Matrix4x4 matrix, float zRotation)
    {
        return Matrix4x4.Multiply(matrix, Matrix4x4.CreateRotationZ(zRotation));
    }
}

public class Transform
{
    public Vector3 Translation { get; set; } = Vector3.Zero;
    public float XRotation { get; set; }
    public float YRotation { get; set; }
    public float ZRotation { get; set; }
    public Vector3 Scale { get; set; } = Vector3.One;


    public static Transform Combine(Transform a, Transform b)
    {
        return new Transform
        {
            Translation = a.Translation + b.Translation,
            XRotation = a.XRotation + b.XRotation,
            YRotation = a.YRotation + b.YRotation,
            ZRotation = a.ZRotation + b.ZRotation,
            Scale = a.Scale + b.Scale
        };
    }

    public Matrix4x4 CreateTransformationMatrix()
    {
        var matrix = Matrix4x4.Identity;
        matrix = Matrix4x4.Multiply(matrix, Matrix4x4.CreateScale(Scale));
        matrix = Matrix4x4.Multiply(matrix, Matrix4x4.CreateTranslation(Translation));
        matrix = Matrix4x4.Multiply(matrix, Matrix4x4.CreateRotationX(XRotation));
        matrix = Matrix4x4.Multiply(matrix, Matrix4x4.CreateRotationY(YRotation));
        matrix = Matrix4x4.Multiply(matrix, Matrix4x4.CreateRotationZ(ZRotation));

        return matrix;
    }

    public static Vector3 Multiply(Matrix4x4 matrices, Vector3 vector4)
    {
        var x = matrices.M11 * vector4.X + matrices.M21 * vector4.Y + matrices.M31 * vector4.Z + matrices.M41;
        var y = matrices.M12 * vector4.X + matrices.M22 * vector4.Y + matrices.M32 * vector4.Z + matrices.M42;
        var z = matrices.M13 * vector4.X + matrices.M23 * vector4.Y + matrices.M33 * vector4.Z + matrices.M43;

        return new Vector3(x, y, z);
    }
}