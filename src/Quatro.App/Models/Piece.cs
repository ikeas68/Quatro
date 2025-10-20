using System;
using System.Windows.Media;

namespace Quatro.Models
{

    public sealed class Piece
    {
        public Piece(bool isLight, bool isWarm, bool isVivid, bool isEarthy)
        {
            IsLight = isLight;
            IsWarm = isWarm;
            IsVivid = isVivid;
            IsEarthy = isEarthy;
            Id = $"{(isLight ? 1 : 0)}{(isWarm ? 1 : 0)}{(isVivid ? 1 : 0)}{(isEarthy ? 1 : 0)}";
            QuadrantBrushes = new[]
            {
            CreateBrush(isLight ? PiecePalette.White : PiecePalette.Black),
            CreateBrush(isWarm ? PiecePalette.Red : PiecePalette.Blue),
            CreateBrush(isVivid ? PiecePalette.Magenta : PiecePalette.Green),
            CreateBrush(isEarthy ? PiecePalette.Brown : PiecePalette.Violet)
        };
        }

        public string Id { get; }

        public bool IsLight { get; }

        public bool IsWarm { get; }

        public bool IsVivid { get; }

        public bool IsEarthy { get; }

        public SolidColorBrush[] QuadrantBrushes { get; }

        private static SolidColorBrush CreateBrush(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        public bool HasAttribute(PieceAttribute attribute) => attribute switch
        {
            PieceAttribute.Light => IsLight,
            PieceAttribute.Warm => IsWarm,
            PieceAttribute.Vivid => IsVivid,
            PieceAttribute.Earthy => IsEarthy,
            _ => throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null)
        };

        public Color GetAttributeColor(PieceAttribute attribute) => attribute switch
        {
            PieceAttribute.Light => IsLight ? PiecePalette.White : PiecePalette.Black,
            PieceAttribute.Warm => IsWarm ? PiecePalette.Red : PiecePalette.Blue,
            PieceAttribute.Vivid => IsVivid ? PiecePalette.Magenta : PiecePalette.Green,
            PieceAttribute.Earthy => IsEarthy ? PiecePalette.Brown : PiecePalette.Violet,
            _ => throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null)
        };
    }

    public enum PieceAttribute
    {
        Light,
        Warm,
        Vivid,
        Earthy
    }

    public static class PiecePalette
    {
        public static readonly Color White = Colors.White;
        public static readonly Color Black = Color.FromRgb(30, 30, 30);
        public static readonly Color Red = Color.FromRgb(216, 41, 59);
        public static readonly Color Blue = Color.FromRgb(35, 87, 202);
        public static readonly Color Magenta = Color.FromRgb(208, 56, 204);
        public static readonly Color Green = Color.FromRgb(26, 143, 67);
        public static readonly Color Brown = Color.FromRgb(148, 92, 46);
        public static readonly Color Violet = Color.FromRgb(105, 60, 174);
    }
}
