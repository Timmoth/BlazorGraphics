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

    public static Vector3 Multiply(this Matrix4x4 matrices, Vector3 vector3)
    {
        var x = matrices.M11 * vector3.X + matrices.M21 * vector3.Y + matrices.M31 * vector3.Z + matrices.M41;
        var y = matrices.M12 * vector3.X + matrices.M22 * vector3.Y + matrices.M32 * vector3.Z + matrices.M42;
        var z = matrices.M13 * vector3.X + matrices.M23 * vector3.Y + matrices.M33 * vector3.Z + matrices.M43;

        return new Vector3(x, y, z);
    }
}