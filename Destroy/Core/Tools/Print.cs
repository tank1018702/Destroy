namespace Destroy
{
    using System;

    public static class Print
    {
        public static void Draw(object msg) => Console.Write(msg);

        public static void Draw(object msg, ConsoleColor foreColor)
        {
            Console.ForegroundColor = foreColor;
            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void Draw(object msg, ConsoleColor foreColor, ConsoleColor backColor)
        {
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void DrawLine(object msg) => Console.WriteLine(msg);

        public static void DrawLine(object msg, ConsoleColor foreColor)
        {
            Console.ForegroundColor = foreColor;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void DrawLine(object msg, ConsoleColor foreColor, ConsoleColor backColor)
        {
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}