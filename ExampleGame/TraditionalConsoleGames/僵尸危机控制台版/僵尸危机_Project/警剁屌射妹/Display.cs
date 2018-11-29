using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static System.Console;
using static 僵尸危机.Data;

namespace 僵尸危机
{
    class Display
    {
        //屏幕初始化
        public Display(string mapName)
        {
            LoadMap(mapName);
            CursorVisible = false;
        }

        //地图长度，宽度
        public int length, width;
        //UI绘制区域
        public static int start = WindowHeight - 3, end = WindowHeight - 2;
        //文件
        public char[,] tempFile;
        //地图
        public string[,] map;
        public string[,] mapBuffer;
        public ConsoleColor[,] foreColor;
        public ConsoleColor[,] backColor;
        //地形
        public static string wall = "■";
        public static string ground = "  ";

        //获取相对路径
        public string GetPath()
        {
            //本程序的项目文件绝对路径
            DirectoryInfo path = new DirectoryInfo(Environment.CurrentDirectory);
            //获取相对于项目文件的路径
            string relativePath = path.Parent.Parent.Parent.FullName;
            return relativePath;
        }

        //(exe文件)获取相对路径
        //public string GetPath()
        //{
        //    //本程序的项目文件绝对路径
        //    DirectoryInfo path = new DirectoryInfo(Environment.CurrentDirectory);
        //    //获取相对于项目文件的路径
        //    string relativePath = path.FullName;
        //    return relativePath;
        //}

        //读取地图
        private void LoadMap(string mapName)
        {
            //地图绝对路径
            string mapPath = GetPath() + @"\" + mapName + ".txt";
            //找文件
            if (File.Exists(mapPath) == false)
            {
                throw new Exception("无法找到该文件,MMP!");
            }
            //读取文件
            string[] result = File.ReadAllLines(mapPath);

            try
            {
                //获取X轴长度
                foreach (string each in result)
                {
                    length++;
                }
                //获取Y轴长度
                char[] oneLine = result[0].ToCharArray();
                foreach (char each in oneLine)
                {
                    if (each != 0)
                    {
                        width++;
                    }
                }
                //所有数组初始化
                tempFile = new char[length, width];
                map = new string[length, width];
                mapBuffer = new string[length, width];
                foreColor = new ConsoleColor[length, width];
                backColor = new ConsoleColor[length, width];

                //文件数组赋值
                for (int x = 0; x < length; x++)
                {
                    char[] c = result[x].ToCharArray();
                    for (int y = 0; y < width; y++)
                    {
                        tempFile[x, y] = c[y];
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("txt地图读取错误！Fuck!");
            }
        }

        //暂存
        public void LockFrame(string[,] copy, string[,] paste)
        {
            for (int x = 0; x < copy.GetLength(0); ++x)
            {
                for (int y = 0; y < copy.GetLength(1); ++y)
                {
                    paste[x, y] = copy[x, y];
                }
            }
        }
        //初始化
        public void Initial()
        {
            //暂存上一帧
            LockFrame(map, mapBuffer);
            //清空地图
            for (int x = 0; x < length; ++x)
            {
                for (int y = 0; y < width; ++y)
                {
                    map[x, y] = "";
                    foreColor[x, y] = ConsoleColor.Gray;
                    backColor[x, y] = ConsoleColor.Black;
                }
            }
        }

        //赋值一切游戏物体
        public void DrawEveryThing(Player player)
        {
            //GameObject
            DrawMap();
            DrawPlayer(player);
            DrawZombie();
            DrawBox();
            DrawBitch();
            //子弹放在最后
            DrawBullets(player);
            //UI
            UICurrentAmmo(player);
            UICurrentHp(player);
            UIKillFuck();
        }

        //赋值地图
        void DrawMap()
        {
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    if (tempFile[x, y] == '#')
                    {
                        map[x, y] = wall;
                    }
                    else if (tempFile[x, y] == ' ')
                    {
                        map[x, y] = ground;
                    }
                }
            }
        }
        //赋值玩家
        void DrawPlayer(Player player)
        {
            //赋值
            map[player.posX, player.posY] = Player.word;
            foreColor[player.posX, player.posY] = player.playerColor;
        }
        //赋值僵尸
        void DrawZombie()
        {
            foreach (var each in zombies)
            {
                //赋值
                map[each.posX, each.posY] = Zombie.word;
                foreColor[each.posX, each.posY] = Zombie.zombieColor;
            }
        }
        //赋值bitch
        void DrawBitch()
        {
            foreach (var each in bitches)
            {
                //赋值
                map[each.posX, each.posY] = Bitch.word;
                foreColor[each.posX, each.posY] = Bitch.bitchColor;
            }
        }
        //赋值盒子
        void DrawBox()
        {
            foreach (var each in boxes)
            {
                //赋值
                map[each.posX, each.posY] = Box.word;
                foreColor[each.posX, each.posY] = Box.boxColor;
            }
        }
        //赋值子弹
        void DrawBullets(Player player)
        {
            List<Bullet> dead = new List<Bullet>();
            foreach (Bullet each in bullets)
            {
                //是否是火箭弹
                bool isRocket = each.damage > 100 ? true : false;
                int nextX = each.bulletX;
                int nextY = each.bulletY;
                //更新下一步
                if (each.bulletDir == Direction.up)
                {
                    nextX -= 1;
                }
                else if (each.bulletDir == Direction.down)
                {
                    nextX += 1;
                }
                else if (each.bulletDir == Direction.left)
                {
                    nextY -= 1;
                }
                else if (each.bulletDir == Direction.right)
                {
                    nextY += 1;
                }
                //火箭弹炸僵尸
                if (map[nextX, nextY] == Zombie.word && isRocket)
                {
                    //死亡范围
                    int[] die = new int[9];
                    //爆炸
                    Boom(nextX, nextY, die);
                    //一群僵尸被炸
                    List<Zombie> zs = new List<Zombie>();
                    foreach (Zombie zombie in zombies)
                    {
                        foreach (int d in die)
                        {
                            //拿唯一键
                            int temp = Play.Pos(zombie.posX, zombie.posY);
                            if (temp == d)
                            {
                                zs.Add(zombie);
                            }
                        }
                    }
                    //死亡僵尸
                    foreach (Zombie z in zs)
                    {
                        //掉血
                        if (z.BeHit(each))
                        {
                            //移除僵尸
                            zombies.Remove(z);
                            kill += 1;
                        }
                    }
                    //检玩家
                    foreach (int d in die)
                    {
                        int temp = Play.Pos(player.posX, player.posY);
                        if (temp == d)
                        {
                            //掉血
                            Player.BeHit(player, each.damage);
                        }
                    }
                    dead.Add(each);
                    continue;
                }
                //火箭弹炸墙壁
                else if (map[nextX, nextY] == wall && isRocket)
                {
                    //死亡范围
                    int[] die = new int[9];
                    //爆炸
                    Boom(nextX, nextY, die);
                    //检玩家
                    foreach (int d in die)
                    {
                        int temp = Play.Pos(player.posX, player.posY);
                        if (temp == d)
                        {
                            //掉血
                            Player.BeHit(player, each.damage);
                        }
                    }
                    dead.Add(each);
                    continue;
                }
                //碰到僵尸
                else if (map[nextX, nextY] == Zombie.word)
                {
                    //僵尸被射
                    Zombie z = null;
                    foreach (Zombie zombie in zombies)
                    {
                        if (zombie.posX == nextX && zombie.posY == nextY)
                        {
                            z = zombie;
                        }
                    }
                    //后退
                    int x = z.posX;
                    int y = z.posY;
                    //僵尸的下一步
                    if (each.bulletDir == Direction.up)
                    {
                        x--;
                    }
                    if (each.bulletDir == Direction.down)
                    {
                        x++;
                    }
                    if (each.bulletDir == Direction.left)
                    {
                        y--;
                    }
                    if (each.bulletDir == Direction.right)
                    {
                        y++;
                    }
                    //如果没碰撞
                    if (x > 0 && y > 0 && x < length && y < width)
                    {
                        //继续飞行
                        if (map[x, y] == ground)
                        {
                            z.posX = x;
                            z.posY = y;
                        }
                    }
                    //掉血
                    if (z.BeHit(each))
                    {
                        //移除僵尸
                        zombies.Remove(z);
                        kill += 1;
                    }
                    //子弹销毁
                    dead.Add(each);
                    continue;
                }
                //碰到墙壁
                if (map[nextX, nextY] == wall)
                {
                    dead.Add(each);
                    continue;
                }
                each.bulletX = nextX;
                each.bulletY = nextY;
                //赋值
                map[each.bulletX, each.bulletY] = each.bulletShape;
                foreColor[each.bulletX, each.bulletY] = each.bulletColor;
            }
            //删除碰壁子弹
            foreach (Bullet each in dead)
            {
                bullets.Remove(each);
            }
        }

        //爆炸特效
        void Boom(int x, int y, int[] die)
        {
            die[0] = Play.Pos(x, y);
            die[1] = Play.Pos(x + 1, y);
            die[2] = Play.Pos(x - 1, y);
            die[3] = Play.Pos(x, y + 1);
            die[4] = Play.Pos(x, y - 1);
            die[5] = Play.Pos(x + 1, y + 1);
            die[6] = Play.Pos(x + 1, y - 1);
            die[7] = Play.Pos(x - 1, y + 1);
            die[8] = Play.Pos(x - 1, y - 1);
            foreach (int each in die)
            {
                Play.XY(each, out int xx, out int yy);
                if (xx < 0 || xx >= length || yy < 0 || yy >= width)
                {
                    continue;
                }
                map[xx, yy] = "炸";
                backColor[xx, yy] = ConsoleColor.Yellow;
            }
            backColor[x, y] = ConsoleColor.Red;
        }

        //杀与干UI
        void UIKillFuck()
        {
            SetCursorPosition(0, start - 1);
            ForegroundColor = ConsoleColor.White;
            BackgroundColor = ConsoleColor.Black;
            Write($"你已经击杀:{kill}个僵尸,干死{fuck}个女人！");
        }
        //血量UI
        void UICurrentHp(Player player)
        {
            SetCursorPosition(0, start);
            BackgroundColor = ConsoleColor.Black;
            //剩余血量比例
            double rest = (double)player.currentHealth / player.health;
            if (rest < 0.2)
            {
                ForegroundColor = ConsoleColor.Red;
            }
            else if (rest < 0.6)
            {
                ForegroundColor = ConsoleColor.Yellow;
            }
            else if (rest <= 1)
            {
                ForegroundColor = ConsoleColor.Green;
            }
            Write($"剩余血量:{rest}");
        }
        //子弹UI
        void UICurrentAmmo(Player player)
        {
            SetCursorPosition(0, end);
            BackgroundColor = ConsoleColor.Black;
            //剩余子弹比例
            double rest = (double)player.weapon.currentCount / player.weapon.bulletsCount;
            if (rest < 0.2)
            {
                ForegroundColor = ConsoleColor.Red;
            }
            else if (rest < 0.6)
            {
                ForegroundColor = ConsoleColor.Yellow;
            }
            else if (rest <= 1)
            {
                ForegroundColor = ConsoleColor.Green;
            }
            Write($"{player.weapon.type} 剩余子弹:{rest}");
        }

        //空子弹时
        public static void OutofAmmo()
        {
            SetCursorPosition(0, end);
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.Red;
            Write("               ");
            Write("Out of Ammo!");
        }
        //空血量时
        public static void OutofHp()
        {
            SetCursorPosition(0, start);
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.Red;
            Write("            ");
            Write("Wasted!");
        }
        //清空UI
        public static void ClearUI()
        {
            BackgroundColor = ConsoleColor.Black;
            for (int i = start - 1; i <= end; i++)
            {
                SetCursorPosition(0, i);
                WriteLine("                                      ");
            }
        }
        //捡到盒子时
        public static void PickedUpBox()
        {
            SetCursorPosition(0, end);
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
            Write("   new Weapon!         ");
            Thread.Sleep(500);
        }
        //干到女人时
        public static void FuckedBitch()
        {
            SetCursorPosition(0, start);
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
            Write("   我TMD真是爽翻了!         ");
            Thread.Sleep(2500);
        }

        //游戏开始
        public static void DrawGameStart()
        {
            CursorVisible = false;
            bool play = false;
            while (play == false)
            {
                Clear();
                SetCursorPosition(42, start - 15);
                ForegroundColor = ConsoleColor.Yellow;
                WriteLine(@"僵尸危机 by  社会你*哥,人蠢B话多");
                ConsoleKey input = ReadKey(true).Key;
                if (input == ConsoleKey.Enter)
                {
                    play = true;
                    SetCursorPosition(42, start - 15);
                    Write("加载地图中...");
                    Thread.Sleep(5000);
                    Clear();
                }
            }
        }
        //游戏胜利
        public static void DrawGameWin()
        {
            Thread.Sleep(2000);
            Clear();
            SetCursorPosition(35, start - 15);
            ForegroundColor = ConsoleColor.Red;
            BackgroundColor = ConsoleColor.Black;
            Write($"你一共杀了{kill}个僵尸,干死{fuck}个女人! 大吉大利,晚上**");
            Thread.Sleep(5000);
        }
        //游戏死亡
        public static void DrawGameOver(bool isjjrw)
        {
            string reason = null;
            if (isjjrw == true)
            {
                reason = "女人把你榨干了!!";
            }
            else
            {
                reason = "僵尸吃了你的小**";
            }
            Thread.Sleep(2000);
            Clear();
            SetCursorPosition(35, start - 15);
            ForegroundColor = ConsoleColor.Red;
            BackgroundColor = ConsoleColor.Black;
            Write($"你一共杀了{kill}个僵尸,干死{fuck}个女人! {reason}");
            Thread.Sleep(5000);
        }


        //刷新画面
        public void FrameUpdate()
        {
            //画图
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    //如果有变动就更新
                    if (map[x, y] != mapBuffer[x, y])
                    {
                        //找到打印位置
                        SetCursorPosition(y * 2, x);
                        //设置前景色，背景色
                        ForegroundColor = foreColor[x, y];
                        BackgroundColor = backColor[x, y];
                        //打印字符
                        Write(map[x, y]);
                    }
                }
            }
        }
    }
}