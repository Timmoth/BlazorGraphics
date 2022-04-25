namespace MauiGraphicsSandbox
{
    public class Animator
    {
        public Transform TransformationData;

        public Animator(Transform transform)
        {
            TransformationData = transform;
        } 

        public float DxRotation { get; set; } = 0;
        public float DyRotation { get; set; } = 0;
        public float DzRotation { get; set; } = 0;

        public void Apply(float dt)
        {
            TransformationData.XRotation += DxRotation * dt; 
            TransformationData.YRotation += DyRotation * dt;
            TransformationData.ZRotation += DzRotation * dt;
        }
    }

}
