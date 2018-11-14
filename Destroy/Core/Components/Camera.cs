namespace Destroy
{
    public class Camera : Component
    {
        public Renderer Canvas;
        public Renderer FOV;
        public Transform CanvasPos;
        public Transform WindowPos;

        public Camera()
        {
            Canvas = new Renderer();
            FOV = new Renderer();
            CanvasPos = new Transform();
            WindowPos = new Transform();
        }
    }
}