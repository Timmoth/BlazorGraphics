using System.Drawing;
using System.Numerics;
using Aptacode.BlazorCanvas;
using BlazorGraphics.Shapes;

namespace BlazorGraphics;

public class LineWave
{
    private readonly List<Shape> Shapes = new();
    PerlinNoise perlinNoise = new(99);

    public LineWave()
    {
        var cubeTranslateGroup = new Group
        {
            Matrix = Matrix4x4.Identity.Translate(new Vector3(125, -250, 100))
        };

        var cubeGroup = new Group
        {
            Animator = new Animator()
            {
                DyRotation = (dt) => 0.015f * dt,
            }
        };

        var size = 30;
        var margin = 40;
        var pointCount = 40;

        for (var x = -20; x < 20; x++)
        {
            int xx = x;
            var points = new Vector3[pointCount];
            GetPoints(xx, pointCount, margin, 1, points);
            var path = new Path3d(points)
            {
                Transform = (dt, nodes) => GetPoints(xx, 40, margin, dt / 30, nodes)
            };

            cubeGroup.Shapes.Add(path);
        }

        cubeTranslateGroup.Shapes.Add(cubeGroup);

        Shapes.Add(cubeTranslateGroup);
    }

    private void GetPoints(int x,  int pointCount, int margin, float dt, Vector3[] points)
    {
        var widthDivisor = 1.0f / margin;
        var heightDivisor = 1.0f / margin;
        var xPos = x * margin;

        var pointsHalf = (int)pointCount / 2;
        for (var y = 0; y < pointCount; y++)
        {
            var xx = x + pointsHalf;
            var yy = y + pointsHalf;
            // Note that the result from the noise function is in the range -1 to 1, but I want it in the range of 0 to 1
            // that's the reason of the strange code
            double v =
                // First octave
                (perlinNoise.Noise(2 * xx * widthDivisor, 2 * yy * heightDivisor, dt / 2) + 1) / 2 * 0.7 +
                // Second octave
                (perlinNoise.Noise(4 * xx * widthDivisor, 4 * yy * heightDivisor, dt / 3) + 1) / 2 * 0.2 +
                // Third octave
                (perlinNoise.Noise(8 * xx * widthDivisor, 8 * yy * heightDivisor, dt / 4) + 1) / 2 * 0.1;

            var b = Math.Min(1, Math.Max(0, v)) * 2000;

            var yPos = (y - pointsHalf) * margin;
            points[y] = new Vector3(xPos, (float)b, yPos);
        }
    }

    public string Background { get; set; } = "#4287f5";

    private float frame = 0;
    public void Draw(BlazorCanvas canvas, Rectangle dirtyRect)
    {
        Clear(canvas, dirtyRect);
        frame+=0.4f;

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

public class PerlinNoise
{
    private const int GradientSizeTable = 256;
    private readonly Random _random;
    private readonly double[] _gradients = new double[GradientSizeTable * 3];
    /* Borrowed from Darwyn Peachey (see references above).
       The gradient table is indexed with an XYZ triplet, which is first turned
       into a single random index using a lookup in this table. The table simply
       contains all numbers in [0..255] in random order. */
    private readonly byte[] _perm = new byte[] {
              225,155,210,108,175,199,221,144,203,116, 70,213, 69,158, 33,252,
                5, 82,173,133,222,139,174, 27,  9, 71, 90,246, 75,130, 91,191,
              169,138,  2,151,194,235, 81,  7, 25,113,228,159,205,253,134,142,
              248, 65,224,217, 22,121,229, 63, 89,103, 96,104,156, 17,201,129,
               36,  8,165,110,237,117,231, 56,132,211,152, 20,181,111,239,218,
              170,163, 51,172,157, 47, 80,212,176,250, 87, 49, 99,242,136,189,
              162,115, 44, 43,124, 94,150, 16,141,247, 32, 10,198,223,255, 72,
               53,131, 84, 57,220,197, 58, 50,208, 11,241, 28,  3,192, 62,202,
               18,215,153, 24, 76, 41, 15,179, 39, 46, 55,  6,128,167, 23,188,
              106, 34,187,140,164, 73,112,182,244,195,227, 13, 35, 77,196,185,
               26,200,226,119, 31,123,168,125,249, 68,183,230,177,135,160,180,
               12,  1,243,148,102,166, 38,238,251, 37,240,126, 64, 74,161, 40,
              184,149,171,178,101, 66, 29, 59,146, 61,254,107, 42, 86,154,  4,
              236,232,120, 21,233,209, 45, 98,193,114, 78, 19,206, 14,118,127,
               48, 79,147, 85, 30,207,219, 54, 88,234,190,122, 95, 67,143,109,
              137,214,145, 93, 92,100,245,  0,216,186, 60, 83,105, 97,204, 52};

    public PerlinNoise(int seed)
    {
        _random = new Random(seed);
        InitGradients();
    }

    public double Noise(double x, double y, double z)
    {
        /* The main noise function. Looks up the pseudorandom gradients at the nearest
           lattice points, dots them with the input vector, and interpolates the
           results to produce a single output value in [0, 1] range. */

        int ix = (int)Math.Floor(x);
        double fx0 = x - ix;
        double fx1 = fx0 - 1;
        double wx = Smooth(fx0);

        int iy = (int)Math.Floor(y);
        double fy0 = y - iy;
        double fy1 = fy0 - 1;
        double wy = Smooth(fy0);

        int iz = (int)Math.Floor(z);
        double fz0 = z - iz;
        double fz1 = fz0 - 1;
        double wz = Smooth(fz0);

        double vx0 = Lattice(ix, iy, iz, fx0, fy0, fz0);
        double vx1 = Lattice(ix + 1, iy, iz, fx1, fy0, fz0);
        double vy0 = Lerp(wx, vx0, vx1);

        vx0 = Lattice(ix, iy + 1, iz, fx0, fy1, fz0);
        vx1 = Lattice(ix + 1, iy + 1, iz, fx1, fy1, fz0);
        double vy1 = Lerp(wx, vx0, vx1);

        double vz0 = Lerp(wy, vy0, vy1);

        vx0 = Lattice(ix, iy, iz + 1, fx0, fy0, fz1);
        vx1 = Lattice(ix + 1, iy, iz + 1, fx1, fy0, fz1);
        vy0 = Lerp(wx, vx0, vx1);

        vx0 = Lattice(ix, iy + 1, iz + 1, fx0, fy1, fz1);
        vx1 = Lattice(ix + 1, iy + 1, iz + 1, fx1, fy1, fz1);
        vy1 = Lerp(wx, vx0, vx1);

        double vz1 = Lerp(wy, vy0, vy1);
        return Lerp(wz, vz0, vz1);
    }

    private void InitGradients()
    {
        for (int i = 0; i < GradientSizeTable; i++)
        {
            double z = 1f - 2f * _random.NextDouble();
            double r = Math.Sqrt(1f - z * z);
            double theta = 2 * Math.PI * _random.NextDouble();
            _gradients[i * 3] = r * Math.Cos(theta);
            _gradients[i * 3 + 1] = r * Math.Sin(theta);
            _gradients[i * 3 + 2] = z;
        }
    }

    private int Permutate(int x)
    {
        const int mask = GradientSizeTable - 1;
        return _perm[x & mask];
    }

    private int Index(int ix, int iy, int iz)
    {
        // Turn an XYZ triplet into a single gradient table index.
        return Permutate(ix + Permutate(iy + Permutate(iz)));
    }

    private double Lattice(int ix, int iy, int iz, double fx, double fy, double fz)
    {
        // Look up a random gradient at [ix,iy,iz] and dot it with the [fx,fy,fz] vector.
        int index = Index(ix, iy, iz);
        int g = index * 3;
        return _gradients[g] * fx + _gradients[g + 1] * fy + _gradients[g + 2] * fz;
    }

    private double Lerp(double t, double value0, double value1)
    {
        // Simple linear interpolation.
        return value0 + t * (value1 - value0);
    }

    private double Smooth(double x)
    {
        /* Smoothing curve. This is used to calculate interpolants so that the noise
          doesn't look blocky when the frequency is low. */
        return x * x * (3 - 2 * x);
    }
}

public static class Noise
{
    public static float[] Calc1D(int width, float scale)
    {
        var values = new float[width];
        for (var i = 0; i < width; i++)
            values[i] = Generate(i * scale) * 128 + 128;
        return values;
    }

    public static float[,] Calc2D(int width, int height, float scale)
    {
        var values = new float[width, height];
        for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                values[i, j] = Generate(i * scale, j * scale) * 128 + 128;
        return values;
    }

    public static float[,,] Calc3D(int width, int height, int length, float scale)
    {
        var values = new float[width, height, length];
        for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                for (var k = 0; k < length; k++)
                    values[i, j, k] = Generate(i * scale, j * scale, k * scale) * 128 + 128;
        return values;
    }

    public static float CalcPixel1D(int x, float scale)
    {
        return Generate(x * scale) * 128 + 128;
    }

    public static float CalcPixel2D(int x, int y, float scale)
    {
        return Generate(x * scale, y * scale) * 128 + 128;
    }

    public static float CalcPixel3D(int x, int y, int z, float scale)
    {
        return Generate(x * scale, y * scale, z * scale) * 128 + 128;
    }

    static Noise()
    {
        _perm = new byte[PermOriginal.Length];
        PermOriginal.CopyTo(_perm, 0);
    }

    public static int Seed
    {
        get => _seed;
        set
        {
            if (value == 0)
            {
                _perm = new byte[PermOriginal.Length];
                PermOriginal.CopyTo(_perm, 0);
            }
            else
            {
                _perm = new byte[512];
                var random = new Random(value);
                random.NextBytes(_perm);
            }

            _seed = value;
        }
    }

    private static int _seed;

    /// <summary>
    /// 1D simplex noise
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static float Generate(float x)
    {
        var i0 = FastFloor(x);
        var i1 = i0 + 1;
        var x0 = x - i0;
        var x1 = x0 - 1.0f;

        var t0 = 1.0f - x0 * x0;
        t0 *= t0;
        var n0 = t0 * t0 * Grad(_perm[i0 & 0xff], x0);

        var t1 = 1.0f - x1 * x1;
        t1 *= t1;
        var n1 = t1 * t1 * Grad(_perm[i1 & 0xff], x1);
        // The maximum value of this noise is 8*(3/4)^4 = 2.53125
        // A factor of 0.395 scales to fit exactly within [-1,1]
        return 0.395f * (n0 + n1);
    }

    /// <summary>
    /// 2D simplex noise
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private static float Generate(float x, float y)
    {
        const float F2 = 0.366025403f; // F2 = 0.5*(sqrt(3.0)-1.0)
        const float G2 = 0.211324865f; // G2 = (3.0-Math.sqrt(3.0))/6.0

        float n0, n1, n2; // Noise contributions from the three corners

        // Skew the input space to determine which simplex cell we're in
        var s = (x + y) * F2; // Hairy factor for 2D
        var xs = x + s;
        var ys = y + s;
        var i = FastFloor(xs);
        var j = FastFloor(ys);

        var t = (i + j) * G2;
        var X0 = i - t; // Unskew the cell origin back to (x,y) space
        var Y0 = j - t;
        var x0 = x - X0; // The x,y distances from the cell origin
        var y0 = y - Y0;

        // For the 2D case, the simplex shape is an equilateral triangle.
        // Determine which simplex we are in.
        int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
        if (x0 > y0) { i1 = 1; j1 = 0; } // lower triangle, XY order: (0,0)->(1,0)->(1,1)
        else { i1 = 0; j1 = 1; }      // upper triangle, YX order: (0,0)->(0,1)->(1,1)

        // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
        // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
        // c = (3-sqrt(3))/6

        var x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
        var y1 = y0 - j1 + G2;
        var x2 = x0 - 1.0f + 2.0f * G2; // Offsets for last corner in (x,y) unskewed coords
        var y2 = y0 - 1.0f + 2.0f * G2;

        // Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
        var ii = Mod(i, 256);
        var jj = Mod(j, 256);

        // Calculate the contribution from the three corners
        var t0 = 0.5f - x0 * x0 - y0 * y0;
        if (t0 < 0.0f) n0 = 0.0f;
        else
        {
            t0 *= t0;
            n0 = t0 * t0 * Grad(_perm[ii + _perm[jj]], x0, y0);
        }

        var t1 = 0.5f - x1 * x1 - y1 * y1;
        if (t1 < 0.0f) n1 = 0.0f;
        else
        {
            t1 *= t1;
            n1 = t1 * t1 * Grad(_perm[ii + i1 + _perm[jj + j1]], x1, y1);
        }

        var t2 = 0.5f - x2 * x2 - y2 * y2;
        if (t2 < 0.0f) n2 = 0.0f;
        else
        {
            t2 *= t2;
            n2 = t2 * t2 * Grad(_perm[ii + 1 + _perm[jj + 1]], x2, y2);
        }

        // Add contributions from each corner to get the final noise value.
        // The result is scaled to return values in the interval [-1,1].
        return 40.0f * (n0 + n1 + n2); // TODO: The scale factor is preliminary!
    }


    private static float Generate(float x, float y, float z)
    {
        // Simple skewing factors for the 3D case
        const float F3 = 0.333333333f;
        const float G3 = 0.166666667f;

        float n0, n1, n2, n3; // Noise contributions from the four corners

        // Skew the input space to determine which simplex cell we're in
        var s = (x + y + z) * F3; // Very nice and simple skew factor for 3D
        var xs = x + s;
        var ys = y + s;
        var zs = z + s;
        var i = FastFloor(xs);
        var j = FastFloor(ys);
        var k = FastFloor(zs);

        var t = (i + j + k) * G3;
        var X0 = i - t; // Unskew the cell origin back to (x,y,z) space
        var Y0 = j - t;
        var Z0 = k - t;
        var x0 = x - X0; // The x,y,z distances from the cell origin
        var y0 = y - Y0;
        var z0 = z - Z0;

        // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
        // Determine which simplex we are in.
        int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
        int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords

        /* This code would benefit from a backport from the GLSL version! */
        if (x0 >= y0)
        {
            if (y0 >= z0)
            { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // X Y Z order
            else if (x0 >= z0) { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 0; k2 = 1; } // X Z Y order
            else { i1 = 0; j1 = 0; k1 = 1; i2 = 1; j2 = 0; k2 = 1; } // Z X Y order
        }
        else
        { // x0<y0
            if (y0 < z0) { i1 = 0; j1 = 0; k1 = 1; i2 = 0; j2 = 1; k2 = 1; } // Z Y X order
            else if (x0 < z0) { i1 = 0; j1 = 1; k1 = 0; i2 = 0; j2 = 1; k2 = 1; } // Y Z X order
            else { i1 = 0; j1 = 1; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // Y X Z order
        }

        // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
        // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
        // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
        // c = 1/6.

        var x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords
        var y1 = y0 - j1 + G3;
        var z1 = z0 - k1 + G3;
        var x2 = x0 - i2 + 2.0f * G3; // Offsets for third corner in (x,y,z) coords
        var y2 = y0 - j2 + 2.0f * G3;
        var z2 = z0 - k2 + 2.0f * G3;
        var x3 = x0 - 1.0f + 3.0f * G3; // Offsets for last corner in (x,y,z) coords
        var y3 = y0 - 1.0f + 3.0f * G3;
        var z3 = z0 - 1.0f + 3.0f * G3;

        // Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
        var ii = Mod(i, 256);
        var jj = Mod(j, 256);
        var kk = Mod(k, 256);

        // Calculate the contribution from the four corners
        var t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
        if (t0 < 0.0f) n0 = 0.0f;
        else
        {
            t0 *= t0;
            n0 = t0 * t0 * Grad(_perm[ii + _perm[jj + _perm[kk]]], x0, y0, z0);
        }

        var t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
        if (t1 < 0.0f) n1 = 0.0f;
        else
        {
            t1 *= t1;
            n1 = t1 * t1 * Grad(_perm[ii + i1 + _perm[jj + j1 + _perm[kk + k1]]], x1, y1, z1);
        }

        var t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
        if (t2 < 0.0f) n2 = 0.0f;
        else
        {
            t2 *= t2;
            n2 = t2 * t2 * Grad(_perm[ii + i2 + _perm[jj + j2 + _perm[kk + k2]]], x2, y2, z2);
        }

        var t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
        if (t3 < 0.0f) n3 = 0.0f;
        else
        {
            t3 *= t3;
            n3 = t3 * t3 * Grad(_perm[ii + 1 + _perm[jj + 1 + _perm[kk + 1]]], x3, y3, z3);
        }

        // Add contributions from each corner to get the final noise value.
        // The result is scaled to stay just inside [-1,1]
        return 32.0f * (n0 + n1 + n2 + n3); // TODO: The scale factor is preliminary!
    }

    private static byte[] _perm;

    private static readonly byte[] PermOriginal = {
            151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
            151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
        };

    private static int FastFloor(float x)
    {
        return (x > 0) ? ((int)x) : (((int)x) - 1);
    }

    private static int Mod(int x, int m)
    {
        var a = x % m;
        return a < 0 ? a + m : a;
    }

    private static float Grad(int hash, float x)
    {
        var h = hash & 15;
        var grad = 1.0f + (h & 7);   // Gradient value 1.0, 2.0, ..., 8.0
        if ((h & 8) != 0) grad = -grad;         // Set a random sign for the gradient
        return (grad * x);           // Multiply the gradient with the distance
    }

    private static float Grad(int hash, float x, float y)
    {
        var h = hash & 7;      // Convert low 3 bits of hash code
        var u = h < 4 ? x : y;  // into 8 simple gradient directions,
        var v = h < 4 ? y : x;  // and compute the dot product with (x,y).
        return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -2.0f * v : 2.0f * v);
    }

    private static float Grad(int hash, float x, float y, float z)
    {
        var h = hash & 15;     // Convert low 4 bits of hash code into 12 simple
        var u = h < 8 ? x : y; // gradient directions, and compute dot product.
        var v = h < 4 ? y : h == 12 || h == 14 ? x : z; // Fix repeats at h = 12 to 15
        return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v);
    }

    private static float Grad(int hash, float x, float y, float z, float t)
    {
        var h = hash & 31;      // Convert low 5 bits of hash code into 32 simple
        var u = h < 24 ? x : y; // gradient directions, and compute dot product.
        var v = h < 16 ? y : z;
        var w = h < 8 ? z : t;
        return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v) + ((h & 4) != 0 ? -w : w);
    }
}