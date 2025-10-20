using System.Collections.Generic;

namespace Quatro.Models
{

    public static class PieceSet
    {
        public static IReadOnlyList<Piece> All { get; } = CreateAll();

        private static IReadOnlyList<Piece> CreateAll()
        {
            var pieces = new List<Piece>(16);
            for (var i = 0; i < 16; i++)
            {
                var light = (i & 1) != 0;
                var warm = (i & 2) != 0;
                var vivid = (i & 4) != 0;
                var earthy = (i & 8) != 0;
                pieces.Add(new Piece(light, warm, vivid, earthy));
            }

            return pieces;
        }
    }
}
