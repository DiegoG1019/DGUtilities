using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DiegoG.MonoGame
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="VerticalCells">The maximum amount of cells in the Y axis</param>
    /// <param name="HorizontalCells">The maximum amount of cells in the X axis</param>
    public record TiledSheet(int CellWidth, int CellHeight, int HorizontalCells, int VerticalCells) : IEnumerable<Rectangle>
    {
        const string valerror = "Must be larger than 0";
        private bool Validate { get; } =
            (CellWidth > 0 ? true : throw new ArgumentOutOfRangeException(nameof(CellWidth), CellWidth, valerror)) &&
            (CellHeight > 0 ? true : throw new ArgumentOutOfRangeException(nameof(CellHeight), CellHeight, valerror)) &&
            (HorizontalCells > 0 ? true : throw new ArgumentOutOfRangeException(nameof(HorizontalCells), HorizontalCells, valerror)) &&
            (VerticalCells > 0 ? true : throw new ArgumentOutOfRangeException(nameof(VerticalCells), VerticalCells, valerror));

        public Rectangle AreaRectangle => new Rectangle(0, 0, HorizontalCells * CellWidth, VerticalCells * CellHeight);
        public Rectangle this[int X, int Y] => X > HorizontalCells
                    ? throw new ArgumentOutOfRangeException(nameof(X), X, $"Must be less than or equal to {nameof(HorizontalCells)}: {HorizontalCells}")
                    : Y > VerticalCells
                    ? throw new ArgumentOutOfRangeException(nameof(Y), Y, $"Must be less than or equal to {nameof(VerticalCells)}: {VerticalCells}")
                    : (new(X * CellWidth, Y * CellHeight, CellWidth, CellHeight));
        public Rectangle this[int Id] => this[Id % CellWidth, Id / CellWidth];

        public int LastID { get; } = CellWidth * CellHeight + CellWidth + CellHeight;
        public int FirstID { get; } = 0;

        public IEnumerator<Rectangle> GetEnumerator()
        {
            for (int i = FirstID; i <= LastID; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
