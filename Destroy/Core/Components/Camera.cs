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

        public void Center(GameObject target)
        {
            Vector2Int point = target.GetComponent<Transform>().Position;
            transform.Position = new Vector2Int(point.X - Width / 2, point.Y - 1 + Height / 2);
        }
    }
}