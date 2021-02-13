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
        public static ControlledTexture2D GetControlledTexture2D(string filename) => (ControlledTexture2D)Texture2D.GetControlled(filename);

        public static SpriteFont GetSpriteFont(string filename) => SpriteFont.Get(filename);
        public static ControlledSpriteFont GetControlledSpriteFont(string filename) => (ControlledSpriteFont)SpriteFont.GetControlled(filename);

        public static SoundEffect GetSoundEffect(string filename) => SoundEffect.Get(filename);
        public static ControlledSoundEffect GetControlledSoundEffect(string filename) => (ControlledSoundEffect)SoundEffect.GetControlled(filename);

        public static Song GetSong(string filename) => Song.Get(filename);
        public static ControlledSong GetControlledSong(string filename) => (ControlledSong)Song.GetControlled(filename);

        static Assets()
        {
            Texture2D = new Texture2DManager();
            SpriteFont = new SpriteFontManager();
            SoundEffect = new SoundEffectManager();
            Song = new SongManager();
        }

    }
}
