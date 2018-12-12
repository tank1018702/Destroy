namespace Destroy.Testing
{
    using System;

    /// <summary>
    /// 感觉这个还有很大优化的空间...
    /// </summary>
    public class RigidController : Script
    {
        public int Speed;

        public RigidController()
        {
            Speed = 10;
            //Camera.main.transform.Position = transform.Position - new Vector2Int(10, 10);
        }
        Vector2 input = Vector2.Zero;
        Vector2 lastInput = Vector2.Zero;
        public override void Update()
        {
            int x = Input.GetDirectInput(KeyCode.A, KeyCode.D);
            int y = Input.GetDirectInput(KeyCode.S, KeyCode.W);
            input = new Vector2(x, y);

            if(input == Vector2.Zero)
            {
                GetComponent<RigidBody>().Stop();
            }
            else if(input != Vector2.Zero && lastInput == Vector2.Zero)
            {
                GetComponent<RigidBody>().FPosition = input.Normalized * 0.5f;
                GetComponent<RigidBody>().SetSpeed(input * Speed);
            }
            else
            {
                GetComponent<RigidBody>().SetSpeed(input * Speed);
            }
            lastInput = input;
        }
    }

    public class CharacterController : Script
    {
        public int Speed;
        public bool UseLegacy;

        public CharacterController()
        {
            Speed = 10;
            UseLegacy = false;
        }

        private float _x = 0, _y = 0;

        public override void Update()
        {
            if (UseLegacy)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    transform.Translate(new Vector2Int(-1, 0));
                }

                if (Input.GetKey(KeyCode.D))
                {
                    transform.Translate(new Vector2Int(1, 0));
                }

                if (Input.GetKey(KeyCode.W))
                {
                    transform.Translate(new Vector2Int(0, 1));
                }

                if (Input.GetKey(KeyCode.S))
                {
                    transform.Translate(new Vector2Int(0, -1));
                }
            }
            else
            {
                int x = Input.GetDirectInput(KeyCode.A, KeyCode.D);
                int y = Input.GetDirectInput(KeyCode.S, KeyCode.W);
                _x += x * Time.DeltaTime * Speed;
                _y += y * Time.DeltaTime * Speed;

                if (x == 0)
                {
                    _x = 0;
                }

                if (y == 0)
                {
                    _y = 0;
                }

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