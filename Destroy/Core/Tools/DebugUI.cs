using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destroy
{
    public static class DebugUIFactroy
    {
        public static void CreateDebugObjs()
        {
            DebugUIBox debugUIBox = CreateDebugUIBox(RendererSystem.cameraStartPos + new Vector2Int(-1, 0)+ new Vector2Int(0, -Camera.main.Height), 
                Camera.main.Height + 2, Camera.main.Width + 2);

            Label label = UIFactroy.CreateLabel(new Vector2Int(15, 0),"DESTROY ENGINE  Debug Mode", 13);
            label.GetComponent<Renderer>().inDebug = true;
            label.GetComponent<Renderer>().Material = new Material(EngineColor.Red, EngineColor.Black);

            debugUIBox.OnSetCamera(Camera.main.transform.Position);
        }

        public static DebugUIBox CreateDebugUIBox(Vector2Int pos, int height, int width)
        {
            GameObject gameObject = new GameObject("UIBox");
            gameObject.transform.Position = pos;

            //添加一个TextBox控件,用于寻找对应的Lable
            DebugUIBox debugUIBox = gameObject.AddComponent<DebugUIBox>();

            #region 创建边框
            int boxWidth = width, boxHeight = height;
            //添加一个方框
            GameObject boxDrawing = new GameObject("BoxDrawing");
            boxDrawing.transform.Position = pos;
            Mesh mesh = boxDrawing.AddComponent<Mesh>();

            List<Vector2Int> meshList = new List<Vector2Int>();
            //添加上下边框的Mesh
            for (int i = 0; i < boxWidth; i++)
            {
                meshList.Add(new Vector2Int(i, 0));
                meshList.Add(new Vector2Int(i, boxHeight - 1));
            }
            //添加左右边框的Mesh
            for (int i = 0; i < boxHeight; i++)
            {
                meshList.Add(new Vector2Int(0, i));
                meshList.Add(new Vector2Int(boxWidth - 1, i));
            }
            mesh.Init(meshList);

            Renderer renderer = boxDrawing.AddComponent<Renderer>();

            //添加边框的贴图
            StringBuilder sb = new StringBuilder();

            //左上角
            sb.Append(BoxDrawingSupply.boxDownRight);
            sb.Append(' ');
            //上部
            for (int i = 0; i < width - 2; i++)
            {
                sb.Append(BoxDrawingSupply.boxHorizontal);
                sb.Append(BoxDrawingSupply.boxHorizontal);
            }
            //右上角
            sb.Append(' ');
            sb.Append(BoxDrawingSupply.boxDownLeft);
            

            for (int i = 0; i < boxHeight - 2; i++)
            {

                sb.Append(BoxDrawingSupply.boxVertical);
                sb.Append(' ');
                sb.Append(' ');
                sb.Append(BoxDrawingSupply.boxVertical);
                
            }

            //左下角
            sb.Append(BoxDrawingSupply.boxUpRight);
            sb.Append(' ');
            //上部
            for (int i = 0; i < width - 2; i++)
            {
                sb.Append(BoxDrawingSupply.boxHorizontal);
                sb.Append(BoxDrawingSupply.boxHorizontal);
            }
            //右下角
            sb.Append(' ');
            sb.Append(BoxDrawingSupply.boxUpLeft);



            renderer.Init(sb.ToString(), -1, EngineColor.Blue,EngineColor.Black);
            renderer.inDebug = true;
            #endregion
            //("wtf:" + textBox.labels[1].GetComponent<Renderer>().Pos_RenderPoint[new Vector2Int(2,0)].Depth);

            return debugUIBox;
        }

        public static GameObject CreateLeftNum(Vector2Int pos,int n)
        {
            GameObject gameObject = new GameObject("UILeftNum");
            gameObject.transform.Position = pos;

            gameObject.AddComponent<Mesh>().Init(new List<Vector2Int>()
            { new Vector2Int(-2, 0), new Vector2Int(-1, 0), new Vector2Int(0, 0) });
            Renderer renderer = gameObject.AddComponent<Renderer>();
            renderer.inDebug = true;
            //进行初始化
            renderer.Init(Print.NumToStrW4(n) + "├ ", -2, EngineColor.Cyan,EngineColor.Black);
            return gameObject;
        }

        public static GameObject CreateDownNum(Vector2Int pos, int n)
        {
            GameObject gameObject = new GameObject("UIDownNum");
            gameObject.transform.Position = pos;

            gameObject.AddComponent<Mesh>().Init(new List<Vector2Int>()
            { new Vector2Int(0, 0), new Vector2Int(1, 0) ,new Vector2Int(0, -1),new Vector2Int(-1, -1),  });
            Renderer renderer = gameObject.AddComponent<Renderer>();
            renderer.inDebug = true;
            //进行初始化
            renderer.Init("┴───" + Print.NumToStrW4(n), -2, EngineColor.Cyan, EngineColor.Black);
            return gameObject;
        }
    }
    public class DebugUIBox : Component
    {
        public List<GameObject> nums = new List<GameObject>();
        public void OnSetCamera(Vector2Int pos)
        {
            if(nums.Count != 0)
            {
                foreach(var v in nums)
                {
                    Destroy(v);
                }
            }
            DebugUIFactroy.CreateLeftNum(transform.Position + new Vector2Int(0, Camera.main.Height), pos.Y);

            DebugUIFactroy.CreateDownNum(transform.Position + new Vector2Int(1, 0), pos.X);
        }
    }
}
