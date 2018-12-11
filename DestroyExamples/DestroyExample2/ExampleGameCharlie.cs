namespace Destroy.Example2
{
    using Destroy;
    using Destroy.Testing;

    [CreatGameObject]
    public class Test : Script
    {
        public override void Update()
        {
            if (NetworkSystem.Client != null && Input.GetKeyDown(KeyCode.C))
            {
                NetworkSystem.Client.Instantiate_RPC(1, new Vector2Int(1, 0));
            }
            if (NetworkSystem.Client != null && Input.GetKeyDown(KeyCode.P))
            {
                GameObject instance = GameObject.Find("玩家");
                NetworkSystem.Client.Destroy(instance);
            }
        }
    }

    public class ExampleGameCharlie
    {
        public class NetworkPlayerController : NetworkScript
        {
            public override void Start()
            {
                if (IsLocal)
                    AddComponent<CharacterController>();
            }
        }

        public static class Factory
        {
            public static GameObject CreatCamera(int charWidth = 2, int height = 30, int width = 30)
            {
                GameObject go = new GameObject("Camera");
                Camera camera = go.AddComponent<Camera>();
                camera.CharWidth = charWidth;
                camera.Height = height;
                camera.Width = width;
                RendererSystem.Init(go);
                return go;
            }
        }
    }
}