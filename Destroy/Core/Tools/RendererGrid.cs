namespace Destroy
{
    using System;

    public struct RendererGrid
    {
        public char Char;
        public ConsoleColor ForeColor;
        public ConsoleColor BackColor;
        public int ColumnSpacing; //列距
        public int RowSpacing;    //行距

        public RendererGrid(char c, ConsoleColor foreColor, ConsoleColor backColor, int columnSpacing = 1, int rowSpacing = 1)
        {
            Char = c;
            ForeColor = foreColor;
            BackColor = backColor;
            ColumnSpacing = columnSpacing;
            RowSpacing = rowSpacing;
        }
    }
}