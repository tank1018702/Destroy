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

    //        Console.CursorVisible = false;
    //    }

        public override void OnCollision(MeshCollider collision)
        {
            Debug.Warning(collision.gameObject.Name);
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

    //        transform.Translate(new Vector2Int(20, -18));
    //        GroupRenderer sr = AddComponent<GroupRenderer>();
    //        sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new StringRenderer("墙吊墙墙墙"), new Vector2Int(-1, 1)));
    //        sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new PosRenderer("墙吊墙墙墙"), new Vector2Int(0, 0)));
    //        sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new StringRenderer("墙吊墙墙墙墙吊墙墙墙"), new Vector2Int(-1, -1)));

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

    //    }
    //}


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
    }

    [CreatGameObject]
    internal class Test : Script
    {
        public GameObject CreatPlayer()
        {
            Factory.CreatCamera();

            NetworkSystem.CreatSelf += () =>
            {
                return gameObject;
            };
            NetworkSystem.CreatOther += () =>
            {
                GameObject other = new GameObject();

                Renderer r = other.AddComponent<Renderer>();
                r.Init("牛");

                return other;
            };
        }

        public override void OnCollision(MeshCollider collision)
        {
            gameObject.Active = false;
            Invoke("Hide", 0.5f);
        }

        public void Hide()
        {
            Factory.CreatCamera();
            NetworkSystem.Init(new Dictionary<int, Instantiate>() { { 1, CreatPlayer } });
        }
    }
}
