namespace Destroy
{
    //TODO Camera初始化问题. 这里暂时加了个单例
    public class Camera : Component
    {
        public int Width;
        public int Height;
        public int CharWidth;

        public static Camera main { get; private set; }

        public Camera()
        {
            Width = 20;
            Height = 20;
            CharWidth = 1;
            main = this;
        }

        public void Center(GameObject target)
        {
            Vector2Int point = target.GetComponent<Transform>().Position;
            transform.Position = new Vector2Int(point.X - Width / 2, point.Y - 1 + Height / 2);
        }
    }
}