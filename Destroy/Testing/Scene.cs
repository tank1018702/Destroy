namespace Destroy.Testing
{
    using System.Collections.Generic;

    public class Scene
    {
        public string Name { get; private set; }
        internal List<GameObject> gameObjects;

        public Scene(string name)
        {
            Name = name;
            gameObjects = new List<GameObject>();
        }
    }
}