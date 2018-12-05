namespace Destroy
{
    using System;

    public class CharacterController : Script
    {
        public int Speed;
        public bool UseLegacy;

        private float timer = 0;
        private float _x = 0, _y = 0;

        public override void Update()
        {
            if (UseLegacy)
            {
                if (Input.GetKey(KeyCode.A))
                    transform.Translate(new Vector2Int(-1, 0));
                if (Input.GetKey(KeyCode.D))
                    transform.Translate(new Vector2Int(1, 0));
                if (Input.GetKey(KeyCode.W))
                    transform.Translate(new Vector2Int(0, 1));
                if (Input.GetKey(KeyCode.S))
                    transform.Translate(new Vector2Int(0, -1));
            }
            else
            {
                int x = Input.GetDirectInput(KeyCode.A, KeyCode.D);
                int y = Input.GetDirectInput(KeyCode.S, KeyCode.W);
                _x += x * Time.DeltaTime * Speed;
                _y += y * Time.DeltaTime * Speed;

                if (x == 0)
                    _x = 0;
                if (y == 0)
                    _y = 0;

                if (Math.Abs(_x) >= 1)
                {
                    transform.Translate(new Vector2Int((int)_x, 0));
                    _x = 0;
                }
                if (Math.Abs(_y) >= 1)
                {
                    transform.Translate(new Vector2Int(0, (int)_y));
                    _y = 0;
                }
            }
        }
    }
}