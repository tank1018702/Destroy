using System;
using System.Collections.Generic;

namespace 僵尸危机
{
    //方向
    public enum Direction
    {
        up,
        down,
        left,
        right
    }

    //武器类型
    public enum WeaponType
    {
        Pistol = 10,
        UZI = 20,
        BitchGun = 50,
        Rocket = 300,
    }

    class Weapon
    {
        public Weapon(WeaponType type, int bulletsCount)
        {
            damage = (int)type;
            this.type = type;
            this.bulletsCount = bulletsCount;
            currentCount = bulletsCount;
        }
        public int damage;
        public int bulletsCount;
        public int currentCount;
        public WeaponType type;
    }

    class Bullet
    {
        public Bullet(int x, int y, Direction dir, Weapon which)
        {
            string shape = null;

            bool pistol = which.type == WeaponType.Pistol;
            bool uzi = which.type == WeaponType.UZI;
            bool rocket = which.type == WeaponType.Rocket;
            bool bitchGun = which.type == WeaponType.BitchGun;

            //pistol
            if (dir == Direction.up && pistol)
            {
                shape = "∶";
            }
            if (dir == Direction.down && pistol)
            {
                shape = "∶";
            }
            if (dir == Direction.left && pistol)
            {
                shape = "¨";
            }
            if (dir == Direction.right && pistol)
            {
                shape = "¨";
            }

            //uzi
            if (dir == Direction.up && uzi)
            {
                shape = "↑";
            }
            if (dir == Direction.down && uzi)
            {
                shape = "↓";
            }
            if (dir == Direction.left && uzi)
            {
                shape = "←";
            }
            if (dir == Direction.right && uzi)
            {
                shape = "→";
            }

            //rocket
            if (dir == Direction.up && rocket)
            {
                shape = "⊙";
            }
            if (dir == Direction.down && rocket)
            {
                shape = "⊙";
            }
            if (dir == Direction.left && rocket)
            {
                shape = "⊙";
            }
            if (dir == Direction.right && rocket)
            {
                shape = "⊙";
            }

            //bitchGun
            if (dir == Direction.up && bitchGun)
            {
                shape = "·";
            }
            if (dir == Direction.down && bitchGun)
            {
                shape = "·";
            }
            if (dir == Direction.left && bitchGun)
            {
                shape = "·";
            }
            if (dir == Direction.right && bitchGun)
            {
                shape = "·";
            }
            bulletX = x;
            bulletY = y;
            bulletShape = shape;
            bulletDir = dir;
            damage = which.damage;
            bulletColor = ConsoleColor.Yellow;
        }

        //子弹的位置
        public int bulletX;
        public int bulletY;
        //子弹形状
        public string bulletShape;
        //子弹颜色
        public ConsoleColor bulletColor;
        //子弹的方向
        public Direction bulletDir;
        //子弹伤害
        public int damage;
    }

    class Box
    {
        static Random random = new Random();

        public int posX;
        public int posY;
        public WeaponType reward;
        //子弹数量
        public static int Ammo;
        public static string word;
        public static ConsoleColor boxColor;

        //被捡
        public void BePick(Player player)
        {
            //捡到盒子！
            Display.PickedUpBox();
            //清空UI
            Display.ClearUI();
            //捡到随机武器
            player.weapon = new Weapon(reward, Ammo);
            //加血
            player.currentHealth += 50;
            if (player.currentHealth > player.health)
            {
                player.currentHealth = player.health;
            }
        }
        //初始化
        public Box(int x, int y, WeaponType type)
        {
            posX = x;
            posY = y;
            reward = type;
            word = "盒";
            boxColor = ConsoleColor.Green;
        }
        //放置
        public static void SetBox(Display d)
        {
            random:
            int x = random.Next(0, d.length - 1);
            int y = random.Next(0, d.width - 1);
            if (d.map[x, y] == Display.ground)
            {
                //字典添加
                WeaponType type = WeaponType.Pistol;
                switch (random.Next(3))
                {
                    case 0:
                        type = WeaponType.Pistol;
                        Ammo = 20;
                        break;
                    case 1:
                        type = WeaponType.UZI;
                        Ammo = 100;
                        break;
                    case 2:
                        type = WeaponType.Rocket;
                        Ammo = 10;
                        break;
                }
                Data.boxes.Add(new Box(x, y, type));
            }
            else
            {
                goto random;
            }
        }
    }

    class Data
    {
        //游戏继续条件(玩家死亡)
        public static bool wasted = false;
        //玩家杀敌
        public static int kill = 0;
        public static int fuck = 0;
        //子弹
        public static List<Bullet> bullets = new List<Bullet>();
        //僵尸群
        public static List<Zombie> zombies = new List<Zombie>();
        //盒子群
        public static List<Box> boxes = new List<Box>();
        //女人群
        public static List<Bitch> bitches = new List<Bitch>();


        //最大僵尸数量
        public static int maxZombieNumber = 30;
        //僵尸移动速度
        public static int zombieSpeed = 0;
        //僵尸生成计时器
        public static int zombieInitTimer = 0;
        //刷僵尸频率
        public static int initZombieCD = 300;

        //玩家初始子弹数量
        public static int initBulletCount = 20;
        //玩家初始武器
        public static WeaponType initWeapon = WeaponType.Pistol;

        //盒子刷新计时器
        public static int boxInitTimer = 0;
        //最大盒子数量
        public static int maxBoxNumber = 3;
        //刷盒子频率
        public static int initBoxCD = 600;

        //最大女人数
        public static int maxBitchNumber = 5;
        //刷女人频率
        public static int initBitchCD = 700;
        //女人生成计时器
        public static int bitchInitTimer = 0;
        //女人速度
        public static int bitchSpeed = 0;
        //死在女人手上
        public static bool jjrw = false;
    }
}