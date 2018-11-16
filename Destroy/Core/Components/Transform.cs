namespace Destroy
{
    using System.Collections.Generic;

    public class Transform : Component
    {
        public Vector2Int Position;
        public CoordinateType Coordinate;
        public Transform Parent;
        private List<Transform> childs;

        public Transform()
        {
            Position = Vector2Int.Zero;
            Coordinate = CoordinateType.Window;
            Parent = null;
            childs = new List<Transform>();
        }

        public void Translate(Vector2Int vector)
        {
            Position += vector;
        }

        public Transform GetChild(string name)
        {
            foreach (var item in childs)
            {
                if (item.gameObject.Name == name)
                    return item;
            }
            return null;
        }

        public Transform GetChild(int id)
        {
            if (id > -1 && id < childs.Count)
                return childs[id];
            else
                return null;
        }
    }
}