namespace Destroy.Test
{
    using System.Collections.Generic;

    public class Template
    {
        public string Id;
        public GameObject Go;
        public int Size;

        public Template(string id, GameObject go, int size)
        {
            Id = id;
            Go = go;
            Size = size;
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public static class ObjectPool
    {
        private static readonly Dictionary<string, List<GameObject>> pools = new Dictionary<string, List<GameObject>>();

        public static void Init(Template template)
        {
            if (pools.ContainsKey(template.Id))
                return;
        }
    }
}