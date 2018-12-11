namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /*
     * 12/7 by Kyasever
     * Renderer通过继承分为了三个组件:
     * StringRenderer 用于渲染一句字符串
     * PosRenderer 用于渲染一个点
     * GroupRenderer 用于渲染一组图形
     * 之后版本要重新优化
     * 
     * 12/8 重写了Renderer的API
     */


    /// <summary>
    /// Material
    /// </summary>
    public struct Material
    {
        public EngineColor ForeColor;
        public EngineColor BackColor;
        /// <summary>
        /// 使用默认背景色的纯色材质
        /// </summary>
        public static Material DefaultColorMaterial =
            new Material(RendererSystem.DefaultColorFore, RendererSystem.DefaultColorBack);

        /// <summary>
        /// 是否是纯色材质
        /// </summary>
        public bool isFullColor;
        /// <summary>
        /// 默认生成纯色材质
        /// </summary>
        public Material(EngineColor ForeColor, EngineColor BackColor)
        {
            isFullColor = true;
            this.ForeColor = ForeColor;
            this.BackColor = BackColor;
        }
    }
    /// <summary>
    /// 贴图类,贴图是一个可以包括换行符的字符串
    /// </summary>
    public class Texture
    {
        public string pic;
        public static Texture Block = new Texture(RendererSystem.Block.str);
        public Texture(string pic)
        {
            this.pic = pic;
        }
    }

    /// <summary>
    /// 着色器,使用的着色器,用于控制Material显示的颜色
    /// </summary>
    public class Shader
    {
        public static Shader Standard = new Shader();
        private Shader()
        {

        }

        /// <summary>
        /// 着色器的渲染函数,只写了标准纯色着色器
        /// </summary>
        /// <param name="material">使用的材质</param>
        /// <param name="dic">处理过的贴图</param>
        /// <param name="depth">渲染的深度</param>
        /// <returns>渲染的结果</returns>
        internal Dictionary<Vector2Int, RenderPoint> Shade(Material material,Dictionary<Vector2Int,string> dic,int depth)
        {
            Dictionary<Vector2Int, RenderPoint> renders = new Dictionary<Vector2Int, RenderPoint>();
            //如果使用的是默认颜色的材质,那么渲染就结束了
            if (material.isFullColor && material.BackColor == RendererSystem.DefaultColorBack && material.ForeColor == RendererSystem.DefaultColorFore)
            {
                foreach (Vector2Int v in dic.Keys)
                {
                    renders.Add(v, new RenderPoint(dic[v], material.ForeColor, material.BackColor, depth));
                }
                return renders;
            }

            //如果是纯色,那么将纯色的颜色渲染上去.
            if (material.isFullColor)
            {
                foreach(Vector2Int v in dic.Keys)
                {
                    renders.Add(v, new RenderPoint(dic[v], material.ForeColor, material.BackColor, depth));
                }
                return renders;
            }
            //渲染失败
            return null;
        }
             
    }


    /// <summary>
    /// 准备重写Renderer,只有一种Renderer pos和vector的对应列表.
    /// 使用字符串初始化,自动进行转换
    /// </summary>
    public class Renderer : Component
    {
        /// <summary>
        /// 为0时脚本显示优先级最高(最后被渲染), 然后向着数轴正方向递减。
        /// 这个深度保存在Renderer中
        /// </summary>
        private int depth;
        public int Depth
        {
            get { return depth; }
            set
            {
                if (Rendering(material, texture, shader))
                {
                    depth = value;
                }
            }
        }


        private Material material;
        /// <summary>
        /// 使用的材质
        /// </summary>
        public Material Material
        {
            get => material;
            set
            {
                if (Rendering(value, texture, shader))
                {
                    material = value;
                }
            }
        }

        private Texture texture;
        /// <summary>
        /// 使用的贴图
        /// </summary>
        public Texture Texture
        {
            get => texture;
            set
            {
                if (Rendering(material, value, shader))
                {
                    texture = value;
                }
            }
        }

        private Shader shader;
        /// <summary>
        /// 使用的着色器
        /// </summary>
        public Shader Shader
        {
            get => shader;
            set
            {
                if (Rendering(material, texture, value))
                {
                    shader = value;
                }
            }
        }

        /// <summary>
        /// 渲染结果
        /// </summary>
        internal Dictionary<Vector2Int, RenderPoint> Pos_RenderPoint = new Dictionary<Vector2Int, RenderPoint>();

        /// <summary>
        /// 通过材质贴图着色器生成renders结果 
        /// 基本符合顶点 - 贴图 - 着色器 渲染管线原理
        /// </summary>
        internal bool Rendering(Material material, Texture texture, Shader shader)
        {
             Mesh mesh = GetComponent<Mesh>();

            //如果是单点Mesh,那么直接输出就完事了.日后在考虑优化..
            if (mesh.isSingleMesh)
            {
                Pos_RenderPoint = new Dictionary<Vector2Int, RenderPoint>();
                Pos_RenderPoint.Add(new Vector2Int(0, 0), new RenderPoint(Print.SubStr(texture.pic, RendererSystem.charWidth), material.ForeColor, material.BackColor, depth));
                return true;
            }

            //从贴图加载字符串信息,并切分成List String
            List<string> strs = Print.DivStr(texture.pic);


            //从Mesh加载顶点信息
            List<Vector2Int> meshPos = mesh.posList;

            int count = strs.Count;
            int s = 0;

            Dictionary<Vector2Int, string> dic = new Dictionary<Vector2Int, string>();
            //根据顶点信息创建hash
            foreach (var v in meshPos)
            {
                //如果有字符串信息,那么根据串信息创建不包含颜色的点
                if(s<count)
                    dic.Add(v, strs[s]);
                //如果字符串信息不足,那么用空格填上
                else
                {
                    dic.Add(v, RendererSystem.Block.str);
                }
                s++;
            }
            //将信息交给Shader处理.完成剩下的工作
            Pos_RenderPoint = shader.Shade(material, dic ,depth);
            return true;
        }


        public Renderer()
        {
        }

        internal override void Initialize()
        {
            //默认显示在最底层
            depth = 100;
            //使用基于系统默认颜色的纯色Material
            material = Material.DefaultColorMaterial;
            //使用标准着色器,这个应该改成反射获取静态类之类的.
            shader = Shader.Standard;
            //使用标准空格作为默认贴图信息
            texture = Texture.Block;
            //初始化渲染器
            Rendering(material, texture, shader);
        }

        public void Init(string str)
        {
            depth = 100;
            material = Material.DefaultColorMaterial;
            shader = Shader.Standard;
            texture = new Texture(str);
            //初始化渲染器
            Rendering(material, texture, shader);
        }

        public void Init(string str, int depth)
        {
            this.depth = depth;
            material = Material.DefaultColorMaterial;
            shader = Shader.Standard;
            texture = new Texture(str);
            //初始化渲染器
            Rendering(material, texture, shader);
        }

        //手动初始化
        public void Init(string str, int depth, EngineColor foreColor,EngineColor backColor)
        {
            this.depth = depth;
            material = new Material(foreColor, backColor);
            //使用标准着色器
            shader = Shader.Standard;
            texture = new Texture(str);
            Rendering(material, texture, shader);
        }
    }
}