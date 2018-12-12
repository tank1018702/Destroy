namespace Destroy
{
    using System.Collections.Generic;

    /// <summary>
    /// Mesh组件 默认生成单点Mesh
    /// </summary>
    public class Mesh : Component
    {
        //使用一个标识符标识这个是不是一个单点Mesh,从而进行优化.
        public bool isSingleMesh = true;

        internal List<Vector2Int> posList { get; private set; }

        public Mesh()
        {
            //默认生成一个单点Mesh,只包含原点
            isSingleMesh = true;
            posList = new List<Vector2Int>();
            posList.Add(new Vector2Int(0, 0));
        }

        public void Sort()
        {
            //使用HashSet去重复. 之后排序
            HashSet<Vector2Int> set = new HashSet<Vector2Int>();
            foreach (var v in posList)
            {
                set.Add(v);
            }
            List<Vector2Int> newList = new List<Vector2Int>();
            foreach (var v in set)
            {
                newList.Add(v);
            }
            newList.Sort();
            posList = newList;
        }

        /// <summary>
        /// 进行多点初始化
        /// </summary>
        public bool Init(List<Vector2Int> list)
        {
            //如果只有一个点,相当于没指定
            if (list.Count == 1)
            {
                if (list[0] == Vector2Int.Zero)
                {
                    isSingleMesh = true;
                    posList = list;
                    Debug.Warning("单点Mesh不需要执行Init操作");
                    return true;
                }
                else
                {
                    Debug.Error("初始化失败,锚点必须包含原点");
                    return false;
                }
            }
            //检测是否包含原点,如果包含那么初始化成功
            foreach (var v in list)
            {
                if (v == Vector2Int.Zero)
                {
                    //如果有零点
                    isSingleMesh = false;
                    posList = list;
                    //更改Mesh的时候进行重新排序
                    Sort();
                    return true;
                }
            }
            Debug.Error("初始化失败,锚点必须包含原点");
            return false;
        }
    }
}