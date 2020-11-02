using System;
using System.Collections.Generic;
using System.Text;

namespace DiegoG.Utilities.Enumerations
{
    public enum Directions
    {
        Up, Down, Left, Right,
        UpRight, UpLeft,
        DownRight, DownLeft
    }
    
    public enum Verbosity { Normal, Debug, Verbose }

    public static class MemberCount
    {
        public static readonly int Directions = Enum.GetNames(typeof(Directions)).Length;
    }

}
