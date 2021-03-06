﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace DiegoG.MonoGame
{
    public static partial class Assets
    {

        private static readonly Texture2DManager Texture2D;
        private static readonly SpriteFontManager SpriteFont;
        private static readonly SoundEffectManager SoundEffect;
        private static readonly SongManager Song;

        public static string DirectoryTexture2D => Texture2D.BaseDirectory;
        public static string DirectorySpriteFont => SpriteFont.BaseDirectory;
        public static string DirectorySoundEffect => SoundEffect.BaseDirectory;
        public static string DirectorySong => Song.BaseDirectory;
        public static Texture2D GetTexture2D(string filename) => Texture2D.Get(filename);
        public static SpriteFont GetSpriteFont(string filename) => SpriteFont.Get(filename);
        public static SoundEffect GetSoundEffect(string filename) => SoundEffect.Get(filename);
        public static Song GetSong(string filename) => Song.Get(filename);

        public static void Update(GameTime gt)
        {
            Texture2D.Update(gt);
            SpriteFont.Update(gt);
            SoundEffect.Update(gt);
            Song.Update(gt);
        }

        static Assets()
        {
            Texture2D = new Texture2DManager();
            SpriteFont = new SpriteFontManager();
            SoundEffect = new SoundEffectManager();
            Song = new SongManager();
        }

    }
}
