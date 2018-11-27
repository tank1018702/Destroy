namespace Destroy
{
    public class Camera : Component
    {
        public int Width;
        public int Height;
        public int CharWidth;

        public Camera()
        {
            Width = 20;
            Height = 20;
            CharWidth = 1;
        }

        public override Component Clone()
        {
            Camera camera = new Camera();
            camera.Name = Name;
            camera.Active = Active;
            camera.Width = Width;
            camera.Height = Height;
            camera.CharWidth = CharWidth;
            return camera;
        }
    }
}