using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Serilog;
using System;
using System.Collections.Generic;

namespace DiegoG.MonoGame
{
    public static class DirectionVector2
    {
        public static Vector2 GetDirection(string i)
        {

            switch (i.ToLower())
            {
                case "up":
                    return Up;
                case "down":
                    return Down;
                case "left":
                    return Left;
                case "right":
                    return Right;
                case "upright":
                    return UpRight;
                case "upleft":
                    return UpLeft;
                case "downright":
                    return DownRight;
                case "downleft":
                    return DownLeft;
            }

            throw new InvalidOperationException("Invalid Direction");

        }

        public static readonly Dictionary<Keys, Vector2> StandardControls = new Dictionary<Keys, Vector2>()
        {
            {Keys.W, Up},
            {Keys.A, Left},
            {Keys.S, Down},
            {Keys.D, Right}
        };

        public static readonly Dictionary<Vector2, Directions> ByVector2 = new Dictionary<Vector2, Directions>();

        public static readonly Dictionary<Directions, Vector2> ByDirections;

        public static readonly Vector2 Up;
        public static readonly Vector2 Down;
        public static readonly Vector2 Left;
        public static readonly Vector2 Right;

        public static readonly Vector2 UpRight;
        public static readonly Vector2 UpLeft;

        public static readonly Vector2 DownRight;
        public static readonly Vector2 DownLeft;

        static DirectionVector2()
        {
            Up = new Vector2(0, -1);
            Down = new Vector2(0, 1);
            Left = new Vector2(-1, 0);
            Right = new Vector2(1, 0);

            UpRight = Up + Right;
            UpLeft = Up + Left;

            DownRight = Down + Right;
            DownLeft = Down + Left;

            ByDirections = new Dictionary<Directions, Vector2>()
            {
                {  Directions.Up, Up },
                {  Directions.Down, Down },
                {  Directions.Left, Left },
                {  Directions.Right, Right },
                {  Directions.UpRight, UpRight },
                {  Directions.UpLeft, UpLeft },
                {  Directions.DownRight, DownRight },
                {  Directions.DownLeft, DownLeft }
            };

            foreach (KeyValuePair<Directions, Vector2> kv in ByDirections)
            {
                Log.Verbose($"Key: {kv.Value}, Value: {kv.Key}");
                ByVector2.Add(kv.Value, kv.Key);
            }
        }

    }
}
