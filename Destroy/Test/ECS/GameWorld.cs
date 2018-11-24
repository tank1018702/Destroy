namespace Destroy.ECS
{
    public class GameWorld
    {
        public static EntityManager manager;

        public static EntityManager CreatOrGetManager()
        {
            if (manager == null)
                manager = new EntityManager();

            return manager;
        }
    }
}