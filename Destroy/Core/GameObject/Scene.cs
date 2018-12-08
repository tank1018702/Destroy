namespace Destroy
{
    using System.Collections.Generic;

    public class Scene
    {
        public string Name { get; private set; }
        private List<GameObject> gameObjects;

        public Scene(string name)
        {
            Name = name;
            gameObjects = new List<GameObject>();
        }
    }
}