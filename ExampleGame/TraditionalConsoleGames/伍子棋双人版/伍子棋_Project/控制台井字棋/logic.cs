using static 控制台五子棋.Data;

namespace 控制台五子棋
{
    //只用作游戏逻辑
    class Logic
    {
        //判断X子棋胜负
        public static void Result()
        {
            //棋盘循环
            for (int x = 1; x <= length - 2; x++)
            {
                for (int y = 1; y <= width - 2; y++)
                {
                    //横，竖，斜的玩家得分
                    int a1 = 1, b1 = 1, c1 = 1, d1 = 1;
                    int a2 = 1, b2 = 1, c2 = 1, d2 = 1;

                    //判断是否n连？
                    for (int i = 1; i < winStep; i++)
                    {
                        //当前子与后一个子
                        State currentState = states[x, y];

                        //玩家1
                        //在棋盘横线之中
                        if (y + i <= width - 2)
                        {
                            State nextState = states[x, y + i];
                            if (currentState == nextState && currentState == State.player1)
                            {
                                a1++;
                            }
                        }
                        //在棋盘竖线之中
                        if (x + i <= length - 2)
                        {
                            State nextState = states[x + i, y];
                            if (currentState == nextState && currentState == State.player1)
                            {
                                b1++;
                            }
                        }
                        //在斜线之中
                        if (x + i < length && y + i < width)
                        {
                            State nextState = states[x + i, y + i];
                            if (currentState == nextState && currentState == State.player1)
                            {
                                c1++;
                            }
                        }
                        //在斜线之中
                        if (x + i < length && y - i > 0)
                        {
                            State nextState = states[x + i, y - i];
                            if (currentState == nextState && currentState == State.player1)
                            {
                                d1++;
                            }
                        }

                        //玩家2
                        //在棋盘横线之中
                        if (y + i <= width - 2)
                        {
                            State nextState = states[x, y + i];
                            if (currentState == nextState && currentState == State.player2)
                            {
                                a2++;
                            }
                        }
                        //在棋盘竖线之中
                        if (x + i <= length - 2)
                        {
                            State nextState = states[x + i, y];
                            if (currentState == nextState && currentState == State.player2)
                            {
                                b2++;
                            }
                        }
                        //在斜线之中
                        if (x + i < length && y + i < width)
                        {
                            State nextState = states[x + i, y + i];
                            if (currentState == nextState && currentState == State.player2)
                            {
                                c2++;
                            }
                        }
                        //在斜线之中
                        if (x + i < length && y - i > 0)
                        {
                            State nextState = states[x + i, y - i];
                            if (currentState == nextState && currentState == State.player2)
                            {
                                d2++;
                            }
                        }
                    }

                    //胜负判断
                    if (a1 == winStep || b1 == winStep || c1 == winStep || d1 == winStep)
                    {
                        winner = 控制台五子棋.Result.玩家1;
                        return;
                    }
                    else if (a2 == winStep || b2 == winStep || c2 == winStep || d2 == winStep)
                    {
                        winner = 控制台五子棋.Result.玩家2;
                        return;
                    }
                }
            }

            //棋盘下满
            if (totalPiece == (length - 2) * (width - 2))
            {
                //平局
                winner = 控制台五子棋.Result.平局;
            }
        }

    }
}