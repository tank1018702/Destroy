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
            transform.Translate(new Vector2Int(0, 0));

            AddComponent<Mesh>();

            MeshCollider mc = AddComponent<MeshCollider>();

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

    [CreatGameObject]
    public class Box : Script
    {
        public override void Start()
        {
            gameObject.Name = "箱子";
            transform.Translate(new Vector2Int(10, -10));

            Mesh mesh = AddComponent<Mesh>();
            mesh.Init(new List<Vector2Int>() { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, -1) });

            MeshCollider mc = AddComponent<MeshCollider>();

            Renderer renderer = AddComponent<Renderer>();
            renderer.Init("一二三四五", 10, EngineColor.Green, EngineColor.Yellow);

            RigidBody rigidBody = AddComponent<RigidBody>();
            rigidBody.Mass = 1f;
        }

        public override void OnCollision(MeshCollider collision)
        {
            Debug.Warning(collision.gameObject.Name);
        }
    }


    [CreatGameObject]
    public class GameMode : Script
    {
        public override void Start()
        {
            UIFactroy.CreateLabel(new Vector2Int(7, 10), "jkl今天天气真不错", 5);
            TextBox textBox = UIFactroy.CreateTextBox(new Vector2Int(3, -7), 10, 10);
            textBox.SetText("TransForm", 1);
            textBox.SetText("TransForm124556", 3);
            textBox.Labels[2].GetComponent<Renderer>().Material = new Material(EngineColor.Green, EngineColor.Yellow);

            //DebugUIFactroy.CreateDebugObjs();


        }
    }



    static class Factory
    {
        public static GameObject CreatCamera(int charWidth = 2, int height = 30, int width = 40)
        {
            GameObject go = new GameObject("Camera");
            Camera camera = go.AddComponent<Camera>();
            camera.transform.Translate(new Vector2Int( -width/2, height/2));
            camera.CharWidth = charWidth;
            camera.Height = height;
            camera.Width = width;
            RendererSystem.Init(go);
            return go;
        }
    }

}
