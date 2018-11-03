using System;
using System.Threading;
using static System.Console;
using static 僵尸危机.Display;
using static 僵尸危机.Data;
using static 僵尸危机.Zombie;
using static 僵尸危机.Box;
using System.Collections.Generic;

namespace 僵尸危机
{
    class Play
    {
        public static Display d;
        public static Player p1;

        // X、Y坐标转成唯一ID
        public static int Pos(int x, int y)
        {
            return x * d.width + y;
        }
        //转回来
        public static void XY(int pos, out int x, out int y)
        {
            //长度
            x = pos / d.width;
            //宽度
            y = pos % d.width;
        }

        //开火
        public static void PlayerShoot(int x, int y)
        {
            //空子弹
            if (p1.weapon.currentCount == 0)
            {
                OutofAmmo();
                return;
            }
            //当前子弹减少
            p1.weapon.currentCount -= 1;
            //生成子弹
            bullets.Add(new Bullet(x, y, p1.playerDir, p1.weapon));
        }
        //开枪
        public static void PlayerFuck(int x, int y)
        {
            List<Bitch> b = new List<Bitch>();
            if (d.map[x, y] == Bitch.word)
            {
                foreach (Bitch each in bitches)
                {
                    if (each.posX == x && each.posY == y)
                    {
                        //如果爽飞了
                        if (each.BeFuck(p1))
                        {
                            b.Add(each);
                            //伤敌一千，自损八百
                            Player.BeHit(p1, 80);
                            //绘制UI
                            ClearUI();
                            FuckedBitch();
                            ClearUI();
                            //如果死亡
                            if (p1.currentHealth <= 0)
                            {
                                jjrw = true;
                            }
                            //给奖励
                            p1.weapon = new Weapon(WeaponType.BitchGun, 20);
                            //长度
                            p1.length += 10;
                            fuck++;
                        }
                    }
                }
            }
            foreach (Bitch each in b)
            {
                bitches.Remove(each);
            }
        }

        //移动
        public static void PlayerMove(Player player)
        {
            //设置按键
            ConsoleKey up = ConsoleKey.W;
            ConsoleKey down = ConsoleKey.S;
            ConsoleKey left = ConsoleKey.A;
            ConsoleKey right = ConsoleKey.D;
            ConsoleKey northWest = ConsoleKey.Q;
            ConsoleKey northEast = ConsoleKey.E;
            ConsoleKey southWest = ConsoleKey.Z;
            ConsoleKey southEast = ConsoleKey.C;
            ConsoleKey shoot = ConsoleKey.J;

            //玩家输入
            ConsoleKey? input = null;
            if (!KeyAvailable)
            {
                return;
            }
            input = ReadKey(true).Key;
            //获取下一步移动位置
            int nextX = player.posX;
            int nextY = player.posY;
            //走位
            if (input == up)
            {
                nextX -= 1;
                player.playerDir = Direction.up;
            }
            else if (input == down)
            {
                nextX += 1;
                player.playerDir = Direction.down;
            }
            else if (input == left)
            {
                nextY -= 1;
                player.playerDir = Direction.left;
            }
            else if (input == right)
            {
                nextY += 1;
                player.playerDir = Direction.right;
            }
            else if (input == northWest)
            {
                nextX -= 1;
                nextY -= 1;
            }
            else if (input == northEast)
            {
                nextX -= 1;
                nextY += 1;
            }
            else if (input == southWest)
            {
                nextX += 1;
                nextY -= 1;
            }
            else if (input == southEast)
            {
                nextX += 1;
                nextY += 1;
            }
            //开火
            else if (input == shoot)
            {
                PlayerShoot(player.posX, player.posY);
                return;
            }
            //其他按键
            else
            {
                return;
            }
            //碰障碍物
            if (d.map[nextX, nextY] == wall)
            {
                return;
            }
            //碰僵尸
            if (d.map[nextX, nextY] == Zombie.word)
            {
                Player.BeHit(p1, damage);
                return;
            }
            //碰女人
            if (d.map[nextX, nextY] == Bitch.word)
            {
                PlayerFuck(nextX, nextY);
                return;
            }
            //碰盒子
            if (d.map[nextX, nextY] == Box.word)
            {
                Box b = null;
                foreach (Box each in boxes)
                {
                    if (each.posX == nextX && each.posY == nextY)
                    {
                        b = each;
                    }
                }
                //捡盒子
                b.BePick(p1);
                //消失
                boxes.Remove(b);
                return;
            }
            //更新移动数据
            player.posX = nextX;
            player.posY = nextY;
        }

        //游戏主循环
        static void Main()
        {
            DrawGameStart();

            //地图初始化
            d = new Display("Map_LostCity");
            //玩家位置初始化
            p1 = new Player(d);

            //初始化
            d.Initial();
            //赋值
            d.DrawEveryThing(p1);
            //渲染
            d.FrameUpdate();

            while (wasted == false)
            {
                //胜利条件
                if (kill >= 100 || fuck >= 10)
                {
                    DrawGameWin();
                    return;
                }
                //逻辑
                PlayerMove(p1);
                //僵尸移动
                if (zombieSpeed >= 20)
                {
                    zombieSpeed = 0;
                    ZombieMove(p1, d);
                }
                //bitch移动
                if (bitchSpeed >= 10)
                {
                    bitchSpeed = 0;
                    Bitch.BitchMove(p1, d);
                }
                //初始化
                d.Initial();
                //赋值
                d.DrawEveryThing(p1);
                //
                d.FrameUpdate();

                Thread.Sleep(30);

                //计时器更新
                zombieInitTimer++;
                zombieSpeed++;
                boxInitTimer++;
                bitchInitTimer++;
                bitchSpeed++;

                //刷一只僵尸
                if (zombieInitTimer >= initZombieCD && zombies.Count < maxZombieNumber)
                {
                    zombieInitTimer = 0;
                    SetZombie(d);
                    //越到后期越难
                    if (hp < 200)
                    {
                        hp += 10;
                    }
                    if (damage < 10)
                    {
                        damage += 1;
                    }
                    //刷怪加快
                    if (initZombieCD > 100)
                    {
                        initZombieCD -= 10;
                    }
                }
                //刷一个盒子
                if (boxInitTimer >= initBoxCD && boxes.Count < maxBoxNumber)
                {
                    boxInitTimer = 0;
                    SetBox(d);
                    //刷盒子加快
                    if (initBoxCD >= 350)
                    {
                        initBoxCD -= 50;
                    }
                }
                //刷一个bitch
                if (bitchInitTimer >= initBitchCD && bitches.Count < maxBitchNumber)
                {
                    bitchInitTimer = 0;
                    Bitch.SetBitch(d);
                }
            }
            DrawGameOver(jjrw);
            return;
        }
    }
}