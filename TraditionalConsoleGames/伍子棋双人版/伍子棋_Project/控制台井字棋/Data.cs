namespace 控制台五子棋
{
    //棋子枚举
    public enum State
    {
        empty = 0,
        player1 = 1,
        player2 = 2
    }
    //地图
    public enum Map
    {
        ground = 0,
        wall = 1,
        focus = 2
    }
    //游戏结果
    public enum Result
    {
        玩家1,
        玩家2,
        平局,
        对局中
    }

    //只管数据结构
    class Data
    {
        //回合数据_____
        //下子数_回合数
        public static int totalPiece = 0;
        //玩家1按键
        public static int player1Key = 1;
        //玩家2按键
        public static int player2Key = 2;
        //比赛结局
        public static Result winner=Result.对局中;
        //几步胜利？
        public static int winStep = 5;
        //玩家焦点位置数组
        public static int[] position = new int[2];

        //地图数据__
        //地图缩进
        public static int vertical_Indent = 10;
        public static int horizontal_indent = 27;
        //棋盘大小
        public static int length = 15, width = 15;
        //地图数组
        public static Map[,] maps = new Map[length, width];
        //棋子数组
        public static State[,] states = new State[length, width];

        //构造方法
        public Data()
        {
            FocusInitial();
            MapInitial();
            StateInitial();
        }
        //焦点初始化
        public static void FocusInitial()
        {
            //地图中点就是焦点
            maps[(length - 1) / 2, (width - 1) / 2] = Map.focus;
            //记录此时横纵坐标
            position[0] = (length - 1) / 2;
            position[1] = (width - 1) / 2;
        }
        //地图初始化
        public static void MapInitial()
        {
            //勾勒地图边缘
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    //上边
                    maps[0, y] = Map.wall;
                    //左边
                    maps[x, 0] = Map.wall;
                    //下边
                    maps[length - 1, y] = Map.wall;
                    //右边
                    maps[x, width - 1] = Map.wall;
                }
            }
        }
        //棋子初始化
        public static void StateInitial()
        {
            //清空棋盘
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    states[x, y] = State.empty;
                }
            }
        }
    }
}