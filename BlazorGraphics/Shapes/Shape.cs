﻿using System;
using System.Numerics;

namespace BlazorGraphics.Shapes;

public abstract class Shape
{
    public Animator Animator { get; set; } = new();
    protected Vector3[] Nodes { get; init; } = Array.Empty<Vector3>();
    public string NodeColor { get; set; } = "black";
    public string EdgeColor { get; set; } = "black";
    public bool FacesOn { get; set; } = true;
    public bool EdgesOn { get; set; } = true;
    public bool NodesOn { get; set; } = true;
    public int Stroke { get; set; } = 2;

    public Matrix4x4 Matrix { get; set; } = Matrix4x4.Identity;

    public Action<float, Vector3[]> Transform { get; set; }

    public virtual (float zIndex, Action<Aptacode.BlazorCanvas.BlazorCanvas>) Draw(Matrix4x4 transform, float dt)
    {
        Transform?.Invoke(dt, Nodes);
        
        var matrix = Matrix4x4.Multiply(Animator.Apply(dt, Matrix), transform);

        var zIndex = 1000f;

        return (zIndex, x => Draw(x, dt, matrix));
    }

    public abstract void Draw(Aptacode.BlazorCanvas.BlazorCanvas x, float dt, Matrix4x4 transform);
}