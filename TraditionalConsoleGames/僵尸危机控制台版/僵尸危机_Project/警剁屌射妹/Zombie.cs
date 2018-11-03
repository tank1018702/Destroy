using System;
using static 僵尸危机.Data;

namespace 僵尸危机
{
    class Zombie
    {
        public static Random random = new Random();
        //放置
        public static void SetZombie(Display d)
        {
            random:
            int x = random.Next(0, d.length - 1);
            int y = random.Next(0, d.width - 1);
            if (d.map[x, y] == Display.ground)
            {
                //集合添加
                zombies.Add(new Zombie(x, y, hp));
            }
            else
            {
                goto random;
            }
        }
        //初始化函数
        public Zombie(int x, int y, int health)
        {
            posX = x;
            posY = y;
            this.health = health;
            currentHealth = health;
        }
        //被打
        public bool BeHit(Bullet bullet)
        {
            bool isDead = false;
            //掉血
            currentHealth -= bullet.damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
            }
            return isDead;
        }

        //僵尸寻路
        public static void ZombieMove(Player player, Display d)
        {
            foreach (Zombie each in zombies)
            {
                int nextX = each.posX;
                int nextY = each.posY;
                //怪物走斜线
                if (player.posX > nextX && player.posY > nextY)
                {
                    nextX++;
                    nextY++;
                }
                else if (player.posX < nextX && player.posY < nextY)
                {
                    nextX--;
                    nextY--;
                }
                else if (player.posX < nextX && player.posY > nextY)
                {
                    nextX--;
                    nextY++;
                }
                else if (player.posY > nextX && player.posY < nextY)
                {
                    nextX++;
                    nextY--;
                }
                else if (player.posX > nextX)
                {
                    nextX++;
                }
                else if (player.posX < nextX)
                {
                    nextX--;
                }
                else if (player.posY > nextY)
                {
                    nextY++;
                }
                else if (player.posY < nextY)
                {
                    nextY--;
                }
                //撞墙
                if (d.map[nextX, nextY] == Display.wall)
                {
                    continue;
                }
                //撞人
                if (d.map[nextX, nextY] == Player.word)
                {
                    Player.BeHit(player, damage);
                    continue;
                }
                //撞僵尸
                if (d.map[nextX, nextY] == word)
                {
                    continue;
                }
                //更新数据
                each.posX = nextX;
                each.posY = nextY;
            }
        }

        public int posX;
        public int posY;
        public int health;
        public int currentHealth;

        public static int damage = 5;
        public static int hp = 60;
        public static string word = "僵";
        public static ConsoleColor zombieColor = ConsoleColor.White;
    }

    //boss
    class Bitch
    {
        public static Random random = new Random();
        //放置
        public static void SetBitch(Display d)
        {
            random:
            int x = random.Next(0, d.length - 1);
            int y = random.Next(0, d.width - 1);
            if (d.map[x, y] == Display.ground)
            {
                //集合添加
                bitches.Add(new Bitch(x, y));
            }
            else
            {
                goto random;
            }
        }

        //初始化
        public Bitch(int x, int y)
        {
            posX = x;
            posY = y;
            pleasure = 0;
        }

        public bool BeFuck(Player player)
        {
            bool isDead = false;
            //起飞
            pleasure += player.length;
            if (pleasure >= 100)
            {
                pleasure = 100;
                isDead = true;
            }
            return isDead;
        }

        public int pleasure;
        public int posX;
        public int posY;
        public static string word = "女";
        public static ConsoleColor bitchColor = ConsoleColor.Magenta;

        //逃跑
        public static void BitchMove(Player player, Display d)
        {
            foreach (Bitch each in bitches)
            {
                int nextX = each.posX;
                int nextY = each.posY;
                //走斜线
                if (player.posX > nextX && player.posY > nextY)
                {
                    nextX--;
                    nextY--;
                }
                else if (player.posX < nextX && player.posY < nextY)
                {
                    nextX++;
                    nextY++;
                }
                else if (player.posX < nextX && player.posY > nextY)
                {
                    nextX++;
                    nextY--;
                }
                else if (player.posY > nextX && player.posY < nextY)
                {
                    nextX--;
                    nextY++;
                }
                else if (player.posX > nextX)
                {
                    nextX--;
                }
                else if (player.posX < nextX)
                {
                    nextX++;
                }
                else if (player.posY > nextY)
                {
                    nextY--;
                }
                else if (player.posY < nextY)
                {
                    nextY++;
                }
                //撞墙
                if (d.map[nextX, nextY] != Display.ground)
                {
                    continue;
                }
                //更新数据
                each.posX = nextX;
                each.posY = nextY;
            }
        }
    }
}