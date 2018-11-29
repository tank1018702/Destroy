using System;

namespace ZombieInfection
{
    public class EntityFactory
    {
        public Entity CreatPlayer()
        {
            Entity player = Gameplay.Instance.CreatEmptyEntity();

            var position = player.AddComponent<Position>();
            var velocity = player.AddComponent<Velocity>();
            var collider = player.AddComponent<Collider>();
            var renderer = player.AddComponent<Renderer>();
            var controller = player.AddComponent<PlayerController>();

            position.Coordinate = MapSingleTon.Instance.GetComponent<Map>();
            position.Point = Vector2.Zero;

            velocity.Speed = 10;

            collider.IsTrigger = false;

            renderer.Str = "吊";
            renderer.ForeColor = ConsoleColor.Blue;
            renderer.BackColor = ConsoleColor.Red;

            controller.Up = KeyCode.W;
            controller.Down = KeyCode.S;
            controller.Left = KeyCode.A;
            controller.Right = KeyCode.D;
            controller.Fire = KeyCode.Space;
            controller.SwitchLeftGun = KeyCode.Q;
            controller.SwitchRightGun = KeyCode.E;

            return player;
        }
    }
}