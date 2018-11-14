namespace Destroy
{
    using System.Collections.Generic;

    public class Transform : Component
    {
        public Vector2Int Position;
        public CoordinateType Coordinate;
        public Transform Parent;
        private List<Transform> childs;

        public void Init(Vector2Int position, CoordinateType coordinate, Transform parent)
        {
            Position = position;
            Coordinate = coordinate;
            Parent = parent;
            childs = new List<Transform>();
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