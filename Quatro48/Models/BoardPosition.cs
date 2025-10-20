namespace Quatro48
{
    public struct BoardPosition
    {
        public BoardPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }

        public int Column { get; }
    }
}
