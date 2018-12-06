namespace Destroy.Example
{
    using Destroy.Test;

    [CreatGameObject]
    public class Game : Script
    {
        public override void Start()
        {
            GameObject go = new GameObject("Camera");
            Camera camera = go.AddComponent<Camera>();
            camera.CharWidth = 2;
            camera.Height = 20;
            camera.Width = 20;
            RendererSystem.Init(go);

            Renderer renderer = AddComponent<Renderer>();
            renderer.Str = "吊";
            AddComponent<Collider>();
            AddComponent<CharacterController>();

            GameObject collider = new GameObject();
            Renderer r = collider.AddComponent<Renderer>();
            r.Str = "墙";
            collider.transform.Position = new Vector2Int(5, 0);
            Collider c = collider.AddComponent<Collider>();
        }

        public override void OnCollision(Collider collision)
        {
            gameObject.Active = false;
            Invoke("S", 0.5f);
        }

        public void S()
        {
            gameObject.Active = true;
            transform.Position = new Vector2Int(0, 0);
        }
    }
}