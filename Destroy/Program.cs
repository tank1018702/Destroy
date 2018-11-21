namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Destroy.Graphics;

    //[CreatGameObject(0, "GameObject", typeof(Collider), typeof(Renderer))]
    //public class A : Script
    //{
    //    public override void Start()
    //    {
    //        GameObject go = new GameObject("Camera");
    //        Camera camera = go.AddComponent<Camera>();
    //        go.GetComponent<Transform>().Position = new Vector2Int(Console.BufferWidth, Console.BufferHeight);
    //        RendererSystem.Init(go);

    //        Collider collider = GetComponent<Collider>();
    //        Renderer renderer = GetComponent<Renderer>();
    //        renderer.Str = "";
    //    }

    //    public override void Update()
    //    {
    //        transform.Position += new Vector2Int(0, -1);
    //    }

    //    public override void OnCollisionEnter(Collider collision)
    //    {
    //    }
    //}

    public class Matrix
    {
        private int[,] items;

        public int Row => items.GetLength(0);
        public int Column => items.GetLength(1);

        public Matrix(int row, int column)
        {
            items = new int[row, column];
        }

        public int this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x > Row - 1 || y > Column - 1)
                    throw new ArgumentOutOfRangeException();
                return items[x, y];
            }
            set
            {
                if (x < 0 || y < 0 || x > Row - 1 || y > Column - 1)
                    throw new ArgumentOutOfRangeException();
                items[x, y] = value;
            }
        }

        public static Matrix operator +(Matrix left, Matrix right)
        {
            if (left.Row != right.Row || left.Column != right.Column)
                throw new Exception("无法相加!");

            int row = left.Row;
            int column = left.Column;
            Matrix matrix = new Matrix(row, column);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    matrix[i, j] = left[i, j] + right[i, j];
                }
            }
            return matrix;
        }

        public static Matrix operator -(Matrix left, Matrix right)
        {
            if (left.Row != right.Row || left.Column != right.Column)
                throw new Exception("无法相减!");
            int row = left.Row;
            int column = left.Column;
            Matrix matrix = new Matrix(row, column);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    matrix[i, j] = left[i, j] - right[i, j];
                }
            }
            return matrix;
        }

        public static Matrix operator *(Matrix left, Matrix right)
        {
            if (left.Column != right.Row)
                throw new Exception("无法相乘!");
            int lr = left.Row;
            int lc = left.Column;
            int rc = right.Column;

            Matrix matrix = new Matrix(lr, rc);
            for (int i = 0; i < lr; i++)
            {
                for (int j = 0; j < rc; j++)
                {
                    for (int k = 0; k < lc; k++)
                        matrix[i, j] = matrix[i, j] + left[i, k] * right[k, j];
                }
            }
            return matrix;
        }
    }

    [CreatGameObject(2)]
    public class C : Script
    {

    }

    [CreatGameObject(0)]
    public class A : Script
    {

    }

    [CreatGameObject(1)]
    public class B : Script
    {

    }

    public class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            runtimeEngine.Run(10);
        }
    }
}