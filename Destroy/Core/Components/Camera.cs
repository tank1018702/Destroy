namespace Destroy
{
    using System;

    public class Camera : Component
    {
        public int BufferWidth;
        public int BufferHeight;

        public Camera()
        {
            BufferWidth = Console.BufferWidth;
            BufferHeight = Console.BufferHeight;
        }

        public override Component Clone()
        {
            Camera camera = new Camera();
            camera.Name = Name;
            camera.Active = Active;
            camera.BufferWidth = BufferWidth;
            camera.BufferHeight = BufferHeight;
            return camera;
        }
    }
}