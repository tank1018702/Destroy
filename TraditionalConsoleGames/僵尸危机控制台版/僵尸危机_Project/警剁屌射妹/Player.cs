using System;

namespace 僵尸危机
{
    class Player
    {

        public Player(Display d)
        {
            //取地图中点
            posX = (d.length - 1) / 2;
            posY = (d.width - 1) / 2;

            //初始武器
            weapon = new Weapon(Data.initWeapon, Data.initBulletCount);
            playerDir = Direction.right;
            playerColor = ConsoleColor.Yellow;
            health = 100;
            currentHealth = health;
        }
        //被打
        public static void BeHit(Player player,int damage)
        {
            //掉血
            player.currentHealth -= damage;
            //事件
            if (player.currentHealth <= 0)
            {
                player.currentHealth = 0;
                Data.wasted = true;
                Display.OutofHp();
            }
        }

        public int posX;
        public int posY;
        public int length = 10;
        public static string word = "吊";
        public ConsoleColor playerColor;
        public int health;
        public int currentHealth;
        public Direction playerDir;
        public Weapon weapon;
    }
}