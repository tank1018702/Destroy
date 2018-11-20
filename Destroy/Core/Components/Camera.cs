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
    }
}