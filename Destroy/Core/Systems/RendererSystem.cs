namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /*
     * 12/7 by Kyasever
     * 新增了复杂渲染系统,暂时使用了清空刷新,当检测到变动时,无条件先输出一行空格清空,然后再输入改动后的内容
     * 之后版本要重新优化
     *
     * TODO:
     * Mesh
     * 所有东西都是从Mesh组件上引申出来的,Mesh是自动生成挂上去的.
     * 内部存储格式 Vector2Int List 必须包含(0,0) 表示点集合与中心点的相对位置
     * PosMesh或者属性表示,表示这个是否是一个单点Mesh.其他组件判断的时候也单独处理
     *
     * MeshCollider 这里面的Vector2Int就是直接获取Mesh就行了
     * 当然也可以作死编辑,无所谓...
     *
     * RigidBody先检测这个东西是不是单点Mesh,如果不是
     * 检测它的MeshCollider,并按着MeshCollier挨个判断过去
     *
     * Material 本质上是Model,Material,Texture的结合体,表现上是一个字符串,可以通过包含换行符来进行多行显示.
     *      对对对,要进行Textures和Material分离,Texture表现上是一个字符串,包含换行符,没了.
     *      Material本质上是对字符串的颜色和格式处理.
     * 例如:
     * Mesh [1,2,3,4],[6]
     * BlockMaterial [绿,蓝,红,白,绿],[青,青,绿,蓝,红]
     * Texture "一二三四五六"
     * Renderer 一二三四
     *          五
     *
     * TODO: \n会进行强制换行,总是会按照矩阵顺序来进行渲染.如果Material比Mesh大,那么截断不需要的部分,如果Material比Mesh小,那么用默认颜色补充
     *
     * MeshRenderer 通过Material来渲染Texture. 改变Mesh,Material或者Texture时都会重新计算
     *     内部存储格式 RenderPoint和Vector2Int list保存
     *     
     *     
     * UI运算 UI组件都有边框 左边框为O| 右边框为|O 可以进行组合 ||
     * 保存一个UI边框的缓冲. 这东西初始化之后就不动了. Render刷新的时候会先调用这个覆盖.
     * String依然使用标准渲染
     */
    /// <summary>
    /// 标准输出点结构.所有的Renderer组件都会被处理为RenderPos的集合
    /// </summary>
    internal struct RenderPoint
    {
        /// <summary>
        /// 这个点的信息,不长于Width
        /// </summary>
        public string str;
        /// <summary>
        /// 前景色
        /// </summary>
        public EngineColor foreColor;
        /// <summary>
        /// 背景色
        /// </summary>
        public EngineColor backColor;

        /// <summary>
        /// 渲染优先级,两个点的判断和加法是取决于优先级的
        /// </summary>
        public int Depth;

        //三项属性相同就视为相等,渲染不需要知道Depth
        public override bool Equals(object obj)
        {
            RenderPoint renderPoint = (RenderPoint)obj;
            return str == renderPoint.str && foreColor == renderPoint.foreColor && backColor == renderPoint.backColor;
        }

        //两个渲染点的相加. 所有覆盖操作都是相加
        public static RenderPoint operator +(RenderPoint left, RenderPoint right)
        {
            //先检测UI,再检测空 UI的空不是空对象
            //如果左方是UI 
            if (left.Depth < 0)
            {
                //如果右方也是UI,那么进行制表符运算和UI标记运算
                //TODO 制表符运算
                if (right.Depth < 0)
                {
                    return right;
                }
                else
                {
                    return left;
                }
            }
            //左边不是 右边是 结果为右边覆盖左边
            else if (right.Depth < 0)
            {
                return right;
            }

            //如果有一侧是空的,那么结果等于另一侧
            if (left.Equals(RendererSystem.Block))
            {
                    return right;
            }
            else if (right.Equals(RendererSystem.Block))
            {
                    return left;
            }



            if(left.Depth < right.Depth)
            {
                //如果优先级高的东西的背景色是默认颜色,那么背景色取另一侧的
                if (left.backColor == RendererSystem.DefaultColorBack)
                {
                    left.backColor = right.backColor;
                }
                return left;
            }
            //二者渲染优先级相等时,取右侧作为结果
            else
            {
                //如果优先级高的东西的背景色是默认颜色,那么背景色取另一侧的
                if (right.backColor == RendererSystem.DefaultColorBack)
                {
                    right.backColor = left.backColor;
                }
                return right;
            }

        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// 使用默认颜色的字符串初始化
        /// </summary>
        public RenderPoint(string str,int depth)
        {
            this.str = str;
            backColor = RendererSystem.DefaultColorBack;
            foreColor = RendererSystem.DefaultColorFore;
            Depth = depth;
        }

        /// <summary>
        /// 使用前景颜色的字符串初始化
        /// </summary>
        public RenderPoint(string str, EngineColor foreColor,int depth)
        {
            this.str = str;
            this.foreColor = foreColor;
            backColor = RendererSystem.DefaultColorBack;
            Depth = depth;
        }

        /// <summary>
        /// 完整的初始化
        /// </summary>
        public RenderPoint(string str, EngineColor foreColor, EngineColor backColor, int depth)
        {
            this.str = str;
            this.foreColor = foreColor;
            this.backColor = backColor;
            Depth = depth;
        }

    }

    /// <summary>
    /// 渲染系统
    /// </summary>
    public static class RendererSystem
    {
        public static EngineColor DefaultColorFore = EngineColor.Gray;
        public static EngineColor DefaultColorBack = EngineColor.Black;
        private static GameObject camera;

        public static int charWidth;

        /// <summary>
        /// 默认的Block就是两格宽的
        /// </summary>
        internal static RenderPoint Block = new RenderPoint("  ",int.MaxValue);



        private static int height;
        private static int width;


        private static RenderPoint[,] renderers;
        private static RenderPoint[,] rendererBuffers;

        //是否开启调试模式.
        public static bool DebugMode = false;
        //摄像机和屏幕的偏移量
        //左上角依然是原点.只是整套系统经过转换之后使用的是第四象限而已.
        public static Vector2Int cameraStartPos = new Vector2Int(10, -2);

        //debug模式下使用变量保存着摄像机缓存数据
        private static int cameraHeight, cameraWidth;

        /// <summary>
        /// 调试模式的初始化. 获得更大的屏幕
        /// </summary>
        /// <param name="camera"></param>
        public static void InitInDebugMode(GameObject camera)
        {
            if (!camera || !camera.Active)
            {
                return;
            }

            Camera cameraComponent = camera.GetComponent<Camera>();
            if (!cameraComponent || !cameraComponent.Active)
            {
                return;
            }

            RendererSystem.camera = camera;

            charWidth = cameraComponent.CharWidth;
            height = cameraComponent.Height + 10;
            cameraHeight = cameraComponent.Height;
            width = cameraComponent.Width + 30;
            cameraWidth = cameraComponent.Width;


            renderers = new RenderPoint[width, height];
            rendererBuffers = new RenderPoint[width, height];

            Console.CursorVisible = false;
            Console.WindowHeight = height + 1;
            Console.BufferHeight = height + 1;
            Console.WindowWidth = width * charWidth + 2;
            Console.BufferWidth = width * charWidth + 2;



            //初始化空渲染点的定义
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < RendererSystem.charWidth; i++)
            {
                sb.Append(' ');
            }
            RendererSystem.Block = new RenderPoint(sb.ToString(), int.MaxValue);


            //将Buffer复制,Render清空为Block
            for (int i = 0; i < renderers.GetLength(0); i++)
            {
                for (int j = 0; j < renderers.GetLength(1); j++)
                {
                    rendererBuffers[i, j] = Block;
                    renderers[i, j] = Block;
                }
            }
        }

        public static void Init(GameObject camera)
        {
            //如果是调试模式,那么重新定向到调试模式渲染系统初始化
            if (DebugMode)
            {
                InitInDebugMode(camera);
                return;
            }

            if (!camera || !camera.Active)
            {
                return;
            }

            Camera cameraComponent = camera.GetComponent<Camera>();
            if (!cameraComponent || !cameraComponent.Active)
            {
                return;
            }

            RendererSystem.camera = camera;

            charWidth = cameraComponent.CharWidth;
            height = cameraComponent.Height;
            width = cameraComponent.Width;
            renderers = new RenderPoint[width, height];
            rendererBuffers = new RenderPoint[width, height];

            Console.CursorVisible = false;
            Console.WindowHeight = height + 1;
            Console.BufferHeight = height + 1;
            Console.WindowWidth = width * charWidth + 2;
            Console.BufferWidth = width * charWidth + 2;

            //初始化空渲染点的定义
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < charWidth; i++)
            {
                sb.Append(' ');
            }
            Block = new RenderPoint(sb.ToString(), int.MaxValue);


            //将Buffer复制,Render清空为Block
            for (int i = 0; i < renderers.GetLength(0); i++)
            {
                for (int j = 0; j < renderers.GetLength(1); j++)
                {
                    rendererBuffers[i, j] = Block;
                    renderers[i, j] = Block;
                }
            }

 
        }

        internal static void Update(List<GameObject> gameObjects)
        {
            //调试模式的话进行重定向
            if(DebugMode)
            {
                UpdateWithDebugMode(gameObjects);
                return;
            }

            if (!camera || !camera.Active)
            {
                return;
            }

            Vector2Int cameraPos = camera.GetComponent<Transform>().Position;

            //将所有Renderer组件上的渲染信息渲染到屏幕上
            foreach (GameObject gameObject in gameObjects)
            {
                if (!gameObject.Active)
                {
                    continue;
                }

                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (!renderer || !renderer.Active)
                {
                    continue;
                }

                //渲染
                Transform transform = renderer.GetComponent<Transform>();
                foreach (var v in renderer.Pos_RenderPoint)
                {
                    //渲染位置
                    Vector2Int vector = transform.Position + v.Key -cameraPos;
                    //如果这个渲染位置没有越界
                    if (vector.X >= 0 && vector.X < width && height - vector.Y >= 0 && height - vector.Y < height)
                    {
                        //使用特定的加法运算,将点的渲染叠加到原点上面去.
                        //更改了一下算法.. 将Y反转
                        renderers[vector.X ,height -vector.Y] +=  v.Value;

                        //Debug.Log(vector);
                    }
                }
            }

            for (int i = 0; i < renderers.GetLength(0); i++)
            {
                for (int j = 0; j < renderers.GetLength(1); j++)
                {
                    RenderPoint renderer = renderers[i, j];
                    RenderPoint bufferRenderer = rendererBuffers[i, j];
                    //Diff 如果二者不相等,那么调用DrawCall
                    if (!renderer.Equals(bufferRenderer))
                    {
                        ConsoleOutPutStandard.Draw(
                            new DrawCall()
                            {
                                X = i * charWidth,
                                Y = j,
                                ForeColor = renderer.foreColor,
                                BackColor = renderer.backColor,
                                Str = renderer.str
                            }
                        );
                    }
                }
            }

            //将Buffer复制,Render清空为Block
            for (int i = 0; i < renderers.GetLength(0); i++)
            {
                for (int j = 0; j < renderers.GetLength(1); j++)
                {
                    rendererBuffers[i, j] = renderers[i, j];
                    renderers[i, j] = Block;
                }
            }
        }

        /// <summary>
        /// 调试模式的初始化. 会调用静态AI作为参数
        /// </summary>
        /// <param name="gameObjects"></param>
        internal static void UpdateWithDebugMode(List<GameObject> gameObjects)
        {
            if (!camera || !camera.Active)
            {
                return;
            }

            Vector2Int cameraPos = camera.GetComponent<Transform>().Position;

            //将所有Renderer组件上的渲染信息渲染到屏幕上
            foreach (GameObject gameObject in gameObjects)
            {
                if (!gameObject.Active)
                {
                    continue;
                }

                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (!renderer || !renderer.Active)
                {
                    continue;
                }

                //如果这个属于调试模式renderer.那么将会直接将Transform渲染到屏幕坐标上面. 和摄像机的位置无关.
                if(renderer.inDebug)
                {
                    Transform transform = renderer.GetComponent<Transform>();
                    foreach (var v in renderer.Pos_RenderPoint)
                    {
                        //渲染位置
                        Vector2Int vector = transform.Position + v.Key - cameraPos + cameraStartPos;
                        //如果这个渲染位置没有越界
                        if (vector.X >= 0 && vector.X < width && height - vector.Y >= 0 && height - vector.Y < height)
                        {
                            //使用特定的加法运算,将点的渲染叠加到原点上面去.
                            //更改了一下算法.. 将Y反转
                            renderers[vector.X, height - vector.Y] += v.Value;
                        }
                    }
                }
                //游戏物体的渲染
                else
                {
                    Transform transform = renderer.GetComponent<Transform>();
                    foreach (var v in renderer.Pos_RenderPoint)
                    {
                        //渲染位置
                        Vector2Int vector = transform.Position + v.Key - cameraPos;
                        //如果这个渲染位置没有越界
                        if (vector.X >= 0 && vector.X < cameraWidth && cameraHeight - vector.Y >= 0 && cameraHeight - vector.Y < cameraHeight)
                        {
                            //使用特定的加法运算,将点的渲染叠加到原点上面去.
                            //更改了一下算法.. 将Y反转
                            renderers[vector.X + cameraStartPos.X, cameraHeight - vector.Y + cameraStartPos.Y] += v.Value;
                        }
                    }
                }

            }


            Console.SetCursorPosition(10,1);

            for (int i = 0; i < renderers.GetLength(0); i++)
            {
                for (int j = 0; j < renderers.GetLength(1); j++)
                {
                    RenderPoint renderer = renderers[i, j];
                    RenderPoint bufferRenderer = rendererBuffers[i, j];
                    //Diff 如果二者不相等,那么调用DrawCall
                    if (!renderer.Equals(bufferRenderer))
                    {
                        ConsoleOutPutStandard.Draw(
                            new DrawCall()
                            {
                                X = i * charWidth,
                                Y = j,
                                ForeColor = renderer.foreColor,
                                BackColor = renderer.backColor,
                                Str = renderer.str
                            }
                        );
                    }
                }
            }

            //将Buffer复制,Render清空为Block
            for (int i = 0; i < renderers.GetLength(0); i++)
            {
                for (int j = 0; j < renderers.GetLength(1); j++)
                {
                    rendererBuffers[i, j] = renderers[i, j];
                    renderers[i, j] = Block;
                }
            }
        }
    }
}
