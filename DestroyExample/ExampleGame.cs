﻿namespace DestroyExample
{
    using System;
    using Destroy;
    using Destroy.Test;

    //[CreatGameObject(0)]
    internal class Test : Script
    {
        public override void Start()
        {




        }
    }

    [CreatGameObject]
    internal class ExampleGame : Script
    {
        public override void Start()
        {
            GameObject go = new GameObject("Camera");
            Camera camera = go.AddComponent<Camera>();
            camera.CharWidth = 2;
            camera.Height = 20;
            camera.Width = 20;
            RendererSystem.Init(go);

            //Renderer renderer = AddComponent<Renderer>();
            //renderer.Str = "吊";
            //AddComponent<Collider>();
            //AddComponent<CharacterController>();

            //GameObject wall = new GameObject();
            //Renderer r = wall.AddComponent<Renderer>();
            //r.Str = "墙";
            //wall.transform.Position = new Vector2Int(5, 0);
            //Collider c = wall.AddComponent<Collider>();

            NetworkSystem.CreatSelf += () =>
            {
                return gameObject;
            };
            NetworkSystem.CreatOther += () =>
            {
                GameObject other = new GameObject();
                Renderer r = other.AddComponent<Renderer>();
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