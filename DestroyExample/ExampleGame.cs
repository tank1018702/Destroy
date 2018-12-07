namespace DestroyExample
{
    using Destroy;
    using Destroy.Test;

    [CreatGameObject]
    internal class ExampleGame: Script
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
            Invoke("Hide", 0.5f);
        }

        public void Hide()
        {
            gameObject.Active = true;
            transform.Position = new Vector2Int(0, 0);
        }
    }
}