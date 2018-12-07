namespace DestroyExample
{
    using System;
    using System.Collections.Generic;
    using Destroy;
    using Destroy.Test;

    [CreatGameObject]
    public class Player : Script
    {
        public override void Start()
        {
            transform.Translate(new Vector2Int(5,-5));
            //PosRenderer sr = AddComponent<PosRenderer>();
            StringRenderer sr = AddComponent<StringRenderer>();
            sr.Str = "吊人ren人人人人人";
            AddComponent<CharacterController>();
            Console.CursorVisible = false;
        }
    }

    [CreatGameObject]
    public class Player2 : Script
    { 
        public override void Start()
        {
            transform.Translate(new Vector2Int(10, -10));
            //PosRenderer sr = AddComponent<PosRenderer>();
            GroupRenderer sr = AddComponent<GroupRenderer>();
            sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new PosRenderer("吊人人"), new Vector2Int(0, 0)));
            sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new PosRenderer("人吊人"), new Vector2Int(0, -1)));
            sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new PosRenderer("人人吊"), new Vector2Int(0, -2)));

            AddComponent<CharacterController>();
        }
    }


    [CreatGameObject]
    internal class ExampleGame: Script
    {
        public override void Start()
        {
            GameObject go = new GameObject("Camera");
            Camera camera = go.AddComponent<Camera>();
            camera.CharWidth = 2;
            camera.Height = 30;
            camera.Width = 30;
            RendererSystem.Init(go);

            PosRenderer renderer = AddComponent<PosRenderer>();
            renderer.Str = "吊";
            AddComponent<Collider>();
            AddComponent<CharacterController>();

            GameObject wall = new GameObject();
            PosRenderer r = wall.AddComponent<PosRenderer>();
            r.Str = "墙";
            wall.transform.Position = new Vector2Int(5, 0);
            Collider c = wall.AddComponent<Collider>();



        }

        public override void OnCollision(Collider collision)
        {
            gameObject.Active = false;
            Invoke("Hide", 0.5f);
        }

        public void Hide()
        {
            gameObject.Active = true;
            transform.Position = new Vector2Int(0, 0);
        }
    }
}