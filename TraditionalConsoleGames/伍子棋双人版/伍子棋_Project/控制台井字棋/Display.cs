using System;
using static 控制台五子棋.Data;
using static System.Console;

namespace 控制台五子棋
{
    //只管根据数据刷新屏幕
    class Display
    {
        //以下方法单独调用
        //玩家1回合
        public static void DrawPlayer1UI()
        {
            ForegroundColor = ConsoleColor.Red;
            //居中
            DrawHorizon(horizontal_indent-1);
            WriteLine("---玩家1的回合----");
        }
        //玩家2回合
        public static void DrawPlayer2UI()
        {
            ForegroundColor = ConsoleColor.Blue;
            //居中
            DrawHorizon(horizontal_indent-1);
            WriteLine("---玩家2的回合----");
        }
        //游戏结束
        public static void DrawEnd()
        {
            //最后刷新
            Show();
            ForegroundColor = ConsoleColor.Cyan;
            WriteLine("游戏结束，胜者是{0}，这局一共下子数{1}", winner, totalPiece);
            ReadLine();
        }

        //主游戏渲染图像
        public static void Show()
        {
            //清空
            Clear();
            //渲染UI优先
            DrawUI();
            //渲染场景
            DrawScene();
        }
        //主界面UI渲染
        public static void DrawUI()
        {
            DrawVertical(7);
            //默认打印红色
            ForegroundColor = ConsoleColor.Red;
            DrawHorizon(26);
            WriteLine("欢迎来到XXOO井字棋");
        }
        //场景渲染
        public static void DrawScene()
        {
            //默认打印白色
            ForegroundColor = ConsoleColor.White;
            //垂直换行
            DrawVertical(1);

            //遍历两个数组，然后打印地图
            for (int x = 0; x < length; x++)
            {
                //水平空格
                DrawHorizon(horizontal_indent - 4);
                for (int y = 0; y < width; y++)
                {
                    State state = states[x, y];
                    Map map = maps[x, y];
                    //单纯光标
                    if (map == Map.focus && state == State.empty)
                    {
                        DrawFocus();
                    }
                    //红底光标
                    else if (map == Map.focus && state == State.player1)
                    {
                        BackgroundColor = ConsoleColor.Red;
                        DrawFocus();
                        BackgroundColor = ConsoleColor.Black;
                    }
                    //蓝底光标
                    else if (map == Map.focus && state == State.player2)
                    {
                        BackgroundColor = ConsoleColor.Blue;
                        DrawFocus();
                        BackgroundColor = ConsoleColor.Black;
                    }
                    //画玩家1_State
                    else if (state == State.player1)
                    {
                        DrawPlayer1();
                    }
                    //画玩家2_State
                    else if (state == State.player2)
                    {
                        DrawPlayer2();
                    }
                    //画墙壁
                    else if (map == Map.wall)
                    {
                        DrawWall();
                    }
                    //画地面
                    else if (map == Map.ground)
                    {
                        DrawGround();
                    }
                }
                //换行
                WriteLine();
            }
        }
        //垂直地图换行
        public static void DrawVertical(int vertical_indent)
        {
            for (int i = 0; i < vertical_indent; i++)
            {
                WriteLine();
            }
        }
        //水平地图空格
        public static void DrawHorizon(int horizontal_indent)
        {
            for (int i = 0; i < horizontal_indent; i++)
            {
                Write("  ");
            }
        }
        //画地
        public static void DrawGround()
        {
            Write("  ");
        }
        //画墙
        public static void DrawWall()
        {
            Write("正");
        }
        //画焦点
        public static void DrawFocus()
        {
            //如果是玩家1的回合
            if (totalPiece % 2 == 0)
            {
                ForegroundColor = ConsoleColor.Magenta;
            }
            else
            {
                ForegroundColor = ConsoleColor.Green;
            }
            Write("吊");
            ForegroundColor = ConsoleColor.White;
        }
        //画玩家1
        public static void DrawPlayer1()
        {
            BackgroundColor = ConsoleColor.Red;
            Write("  ");
            BackgroundColor = ConsoleColor.Black;
        }
        //画玩家2
        public static void DrawPlayer2()
        {
            BackgroundColor = ConsoleColor.Blue;
            Write("  ");
            BackgroundColor = ConsoleColor.Black;
        }
    }
}