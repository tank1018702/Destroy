namespace Destroy.Example2
{
    using System.Collections.Generic;
    using Destroy;
    using Destroy.Testing;

    public class Bullet : NetworkScript
    {
        float timer = 0;
        public override void Update()
        {
            timer += Time.DeltaTime;
            if (timer >= 0.1f && IsLocal)
            {
                timer = 0;
                transform.Translate(new Vector2Int(1, 0));
            }
        }
    }

    [CreatGameObject]
    public class Test : Script
    {
        public override void Start()
        {
            Factory.CreatCamera();
            //注册Prefab
            NetworkSystem.Init(new Dictionary<int, Instantiate>
            {
                { 1, () =>
                    {
                        GameObject go = new GameObject("秒杀");
                        go.AddComponent<Mesh>();
                        Renderer renderer= go.AddComponent<Renderer>();
                        renderer.Init("吊");
                        go.AddComponent<NetworkPlayerController>();
                        return go;
                    }
                }
                ,
                {
                    2,
                    () =>
                    {
                        GameObject bullet = new GameObject("子弹");
                        bullet.AddComponent<Mesh>();
                        var renderer = bullet.AddComponent<Renderer>();
                        renderer.Init("蛋");
                        bullet.AddComponent<Bullet>();
                        return bullet;
                    }
                }
            });
        }

        public override void Update()
        {
            if (NetworkSystem.Client != null && Input.GetKeyDown(KeyCode.C))
            {
                NetworkSystem.Client.Instantiate_RPC(1, new Vector2Int(1, 0));
            }
            if (NetworkSystem.Client != null && Input.GetKeyDown(KeyCode.P))
            {
                GameObject instance = GameObject.Find("秒杀");
                NetworkSystem.Client.Destroy(instance);
            }
        }
    }

    public class NetworkPlayerController : NetworkScript
    {
        public override void Start()
        {
            if (IsLocal)
                AddComponent<CharacterController>();
        }

        public override void Update()
        {
            if (NetworkSystem.Client != null && Input.GetKeyDown(KeyCode.F))
            {
                NetworkSystem.Client.Instantiate_RPC(2, transform.Position);
            }
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