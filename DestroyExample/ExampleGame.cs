/*
namespace Destroy.Example
{
    using System;
    using System.Collections.Generic;
    using Destroy;
    using Destroy.Testing;

    /// <summary>
    /// 还需要彻底封装一次这些东西...
    /// </summary>
    [CreatGameObject(0)]
    public class Player : Script
    {
        public override void Start()
        {
            Factory.CreatCamera();
            Console.CursorVisible = false;

            gameObject.Name = "主角玩家";
            transform.Translate(new Vector2Int(5, -5));

            AddComponent<Mesh>();
            MeshCollider mc = AddComponent<MeshCollider>();
            mc.Init();

            Renderer renderer = AddComponent<Renderer>();
            renderer.Init("吊");

            AddComponent<RigidController>();

            RigidBody rigidBody = AddComponent<RigidBody>();
            rigidBody.Mass = 1000f;


        }

        public override void OnCollision(MeshCollider collision)
        {
            Debug.Warning(collision.gameObject.Name);
        }
    }

    //[CreatGameObject]
    //internal class TestMsg : Script
    //{
    //    NetworkServer server;
    //    NetworkClient client;

    //    public override void Start()
    //    {
    //        if (int.Parse(Console.ReadLine()) == 1)
    //        {
    //            server = new NetworkServer(8848);
    //            server.Start();
    //            server.OnConnected += socket =>
    //            {
    //                byte[] data = System.Text.Encoding.UTF8.GetBytes("hello");
    //                server.Send(socket, 0, 0, data);
    //            };
    //        }
    //        else
    //        {
    //            client = new NetworkClient(NetworkUtils.LocalIPv4Str, 8848);
    //            client.Start();
    //            client.Register(0, 0, data =>
    //            {
    //                string str = System.Text.Encoding.UTF8.GetString(data);
    //                Console.WriteLine(str);
    //            });
    //        }
    //    }

    //    public override void Update()
    //    {
    //        server?.Update();
    //        client?.Update();
    //    }
    //}

    class NetworkPlayerController : NetworkScript
    {
        public override void Start()
        {
            if (IsLocal)
                AddComponent<CharacterController>();
        }
    }

    [CreatGameObject]
    public class Box : Script
    {
        public override void Start()
        {
            gameObject.Name = "箱子";
            transform.Translate(new Vector2Int(10, -10));

            Mesh mesh = AddComponent<Mesh>();
            mesh.Init(new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0,- 1) });

            MeshCollider mc = AddComponent<MeshCollider>();
            mc.Init();

            Renderer renderer = AddComponent<Renderer>();
            renderer.Init("一二三四五",10,EngineColor.Green,EngineColor.Yellow);

            RigidBody rigidBody = AddComponent<RigidBody>();
            rigidBody.Mass = 1f;
        }

        public override void OnCollision(MeshCollider collision)
        {
            Debug.Warning(collision.gameObject.Name);
        }
    }


    [CreatGameObject]
    public class GameMode:Script
    {
        public override void Start()
        {

        }
    }



    static class Factory
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

}
*/