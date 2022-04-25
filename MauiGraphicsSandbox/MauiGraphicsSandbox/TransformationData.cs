using System.Numerics;

namespace MauiGraphicsSandbox
{
    public class Transform
    {
        public Vector3 Translation { get; set; } = Vector3.Zero;
        public float XRotation { get; set; } = 0;
        public float YRotation { get; set; } = 0;
        public float ZRotation { get; set; } = 0;
        public Vector3 Scale { get; set; } = Vector3.One;

        public static Transform Combine(Transform a, Transform b)
        {
            return new Transform()
            {
                Translation = a.Translation + b.Translation,
                XRotation = a.XRotation + b.XRotation,
                YRotation = a.YRotation + b.YRotation,
                ZRotation = a.ZRotation + b.ZRotation,
                Scale = a.Scale + b.Scale,
            };
        }


        public Matrix4x4 CreateTransformationMatrix()
        {
            var matrix = Matrix4x4.Identity;
            matrix = Matrix4x4.Multiply(matrix, Matrix4x4.CreateScale(Scale));
            matrix = Matrix4x4.Multiply(matrix, Matrix4x4.CreateRotationX(XRotation));
            matrix = Matrix4x4.Multiply(matrix, Matrix4x4.CreateRotationY(YRotation));
            matrix = Matrix4x4.Multiply(matrix, Matrix4x4.CreateRotationZ(ZRotation));
            matrix = Matrix4x4.Multiply(matrix, Matrix4x4.CreateTranslation(Translation));

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

}
