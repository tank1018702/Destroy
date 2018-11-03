using System;
using static System.Console;
using static 控制台五子棋.Display;
using static 控制台五子棋.Data;
using static 控制台五子棋.Logic;

namespace 控制台五子棋
{
    //只管实现更新数据，调用方法。
    class Play
    {
        //游戏数据更新(移动，放置)
        private static void GameDataControl(int turn)
        {
            //按键
            ConsoleKey up = ConsoleKey.Enter,
            down = ConsoleKey.Enter, left = ConsoleKey.Enter, right = ConsoleKey.Enter, place = ConsoleKey.Enter;
            //按键设置
            if (turn == 1)
            {
                up = ConsoleKey.W;
                down = ConsoleKey.S;
                left = ConsoleKey.A;
                right = ConsoleKey.D;
                place = ConsoleKey.Spacebar;
            }
            else if (turn == 2)
            {
                up = ConsoleKey.UpArrow;
                down = ConsoleKey.DownArrow;
                left = ConsoleKey.LeftArrow;
                right = ConsoleKey.RightArrow;
                place = ConsoleKey.Enter;
            }
            //获取玩家输入
            ConsoleKeyInfo keyinfo = ReadKey();
            ConsoleKey input = keyinfo.Key;
            //空地恢复
            maps[position[0], position[1]] = Map.ground;

            if (input == up)
            {
                //没超上边界
                if (position[0] != 1)
                {
                    //上移
                    position[0]--;
                }
            }
            else if (input == down)
            {
                //没超下边界
                if (position[0] != length - 2)
                {
                    //下移
                    position[0]++;
                }
            }
            else if (input == left)
            {
                //没超左边界
                if (position[1] != 1)
                {
                    //左移
                    position[1]--;
                }
            }
            else if (input == right)
            {
                //没超右边界
                if (position[1] != width - 2)
                {
                    //右移
                    position[1]++;
                }
            }
            else if (input == place)
            {
                //下棋
                //获取当前位置
                State currentLocation = states[position[0], position[1]];
                if (currentLocation == State.empty && turn == 1)
                {
                    //更新棋子数据
                    states[position[0], position[1]] = State.player1;
                    //下棋数+1
                    totalPiece++;
                }
                else if (currentLocation == State.empty && turn == 2)
                {
                    //更新棋子数据
                    states[position[0], position[1]] = State.player2;
                    //下棋数+1
                    totalPiece++;
                }
            }
            //焦点更新
            maps[position[0], position[1]] = Map.focus;
        }

        //开始
        private static void Start()
        {
            //Data初始化
            new Data();

            //游戏主循环
            while (winner == Result.对局中)
            {
                //刷新画面
                Show();
                //回合交替(0是玩家1先手，1是玩家2)
                //回合交替totalPiece % 2 == 0
                if (totalPiece % 2 == 0)
                {
                    //渲染玩家UI
                    DrawPlayer1UI();
                    //玩家1回合
                    GameDataControl(player1Key);
                }
                else
                {
                    //渲染玩家UI
                    DrawPlayer2UI();
                    //玩家2回合
                    GameDataControl(player2Key);
                }
                //胜负检测
                Result();
            }

            //游戏结束
            DrawEnd();
        }

        private static void Main()
        {
            //开始
            Start();
        }
    }
}