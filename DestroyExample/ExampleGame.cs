namespace DestroyExample
{
    using System;
    using System.Collections.Generic;
    using Destroy;
    using Destroy.Test;

    [CreatGameObject(0)]
    public class Player : Script
    {
        public override void Start()
        {
            Factory.CreatCamera();

            transform.Translate(new Vector2Int(5, -5));
            StringRenderer sr = AddComponent<StringRenderer>();
            sr.Str = "吊人ren人人人人人";
            AddComponent<CharacterController>();
            Console.CursorVisible = false;
        }
    }

    [CreatGameObject]
    public class Player3 : Script
    {
        public override void Start()
        {
            transform.Translate(new Vector2Int(6, -6));
            StringRenderer sr = AddComponent<StringRenderer>();
            sr.Str = "12345678912123123123123123";
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
            GroupRenderer sr = AddComponent<GroupRenderer>();
            sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new StringRenderer("吊人人"), new Vector2Int(-1, 1)));
            sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new PosRenderer("吊"), new Vector2Int(0, 0)));
            sr.list.Add(new KeyValuePair<Renderer, Vector2Int>(new StringRenderer("人人吊"), new Vector2Int(-1, -1)));
            AddComponent<CharacterController>();
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
    }

    //[CreatGameObject]
    internal class ExampleGame : Script
    {
        public override void Start()
        {
            Factory.CreatCamera();

            NetworkSystem.CreatSelf += () =>
            {
                return gameObject;
            };
            NetworkSystem.CreatOther += () =>
            {
                GameObject other = new GameObject();
                PosRenderer r = other.AddComponent<PosRenderer>();
                r.Str = "牛";
                return other;
            };
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