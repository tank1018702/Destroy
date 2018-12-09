namespace Destroy.Example
{
    using System;
    using System.Collections.Generic;
    using Destroy;
    using Destroy.Testing;

    //[CreatGameObject(0)]
    //public class Player : Script
    //{
    //    public override void Start()
    //    {
    //        gameObject.Name = "主角玩家";

    //        Factory.CreatCamera();

    //        transform.Translate(new Vector2Int(5, -5));
    //        StringRenderer sr = AddComponent<StringRenderer>();
    //        sr.Str = "吊";
    //        //AddComponent<CharacterController>();
    //        AddComponent<RigidController>();

    //        RigidBody rigidBody = AddComponent<RigidBody>();
    //        rigidBody.Mass = 1000f;


    //        MeshCollider mc = AddComponent<MeshCollider>();
    //        mc.Init();

    //        foreach (var v in GetComponent<MeshCollider>().posList)
    //        {
    //            Debug.Error(v.ToString());
    //        }

    //        Console.CursorVisible = false;
    //    }

    //    public override void OnCollision(Collider collision)
    //    {
    //        Debug.Warning(collision.gameObject.Name);
    //    }
    //}

    ////[CreatGameObject]
    ////public class Player3 : Script
    ////{
    ////    public override void Start()
    ////    {
    ////        transform.Translate(new Vector2Int(6, -6));
    ////        StringRenderer sr = AddComponent<StringRenderer>();
    ////        sr.Str = "12345678912123123123123123";
    ////        AddComponent<CharacterController>();
    ////        Console.CursorVisible = false;
    ////    }
    ////}

    //[CreatGameObject]
    //public class Wall : Script
    //{ 
    //    public override void Start()
    //    {
    //        gameObject.Name = "墙";

    //        transform.Translate(new Vector2Int(10, -10));
    //        GroupRenderer sr = AddComponent<GroupRenderer>();
    //        sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new StringRenderer("墙吊墙墙墙"), new Vector2Int(-1, 1)));
    //        sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new PosRenderer("墙吊墙墙墙"), new Vector2Int(0, 0)));
    //        sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new StringRenderer("墙吊墙墙墙墙吊墙墙墙"), new Vector2Int(-1, -1)));
    //        StaticCollider sc = AddComponent<StaticCollider>();
    //        sc.InitWithRenderer(sr);
    //    }
    //}

    //[CreatGameObject]
    //public class ActiveWall : Script
    //{
    //    public override void Start()
    //    {
    //        gameObject.Name = "NB墙";

    //        transform.Translate(new Vector2Int(20, -18));
    //        GroupRenderer sr = AddComponent<GroupRenderer>();
    //        sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new StringRenderer("墙吊墙墙墙"), new Vector2Int(-1, 1)));
    //        sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new PosRenderer("墙吊墙墙墙"), new Vector2Int(0, 0)));
    //        sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new StringRenderer("墙吊墙墙墙墙吊墙墙墙"), new Vector2Int(-1, -1)));

    //        MeshCollider mc = AddComponent<MeshCollider>();
    //        mc.Init();

    //        RigidBody rigidBody = AddComponent<RigidBody>();
    //        rigidBody.Mass = 100f;

    //    }
    //}

    //[CreatGameObject]
    //public class Box:Script
    //{
    //    public override void Start()
    //    {
    //        gameObject.Name = "箱子";

    //        transform.Translate(new Vector2Int(4, -15));
    //        StringRenderer sr = AddComponent<StringRenderer>();
    //        sr.Str = "箱";

    //        RigidBody rigidBody = AddComponent<RigidBody>();
    //        rigidBody.Mass = 0.5f;

    //        MeshCollider mc = AddComponent<MeshCollider>();
    //        mc.Init();

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
            GameObject player = new GameObject();
            player.AddComponent<CharacterController>();
            PosRenderer posRenderer = player.AddComponent<PosRenderer>();
            posRenderer.Str = "打";
            return player;
        }

        public override void Start()
        {
            Factory.CreatCamera();

            NetworkSystem.Init(new Dictionary<int, Instantiate>() { { 1, CreatPlayer } });
        }
    }
}