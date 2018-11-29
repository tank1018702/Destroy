using System;

namespace ZombieInfection
{
    public static class Print
    {
        public static void Draw(string str)
        {
            Console.Write(str);
        }

        public static void Draw(string str, ConsoleColor foreColor, ConsoleColor backColor)
        {
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            Console.Write(str);
            Console.ForegroundColor = default(ConsoleColor);
            Console.BackgroundColor = default(ConsoleColor);
        }
    }
}