using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Serilog;
using System;

namespace DiegoG.MonoGame
{
    public static partial class Assets
    {
        public struct ManagedTexture2D
        {
            public string Filename { get; private set; }

            public Texture2D Texture2D => Assets.Texture2D.Get(Filename);

            public static implicit operator Texture2D(ManagedTexture2D v) => v.Texture2D;
            public ManagedTexture2D(string filename) => Filename = filename;
        }

        public struct ManagedSpriteFont
        {
            public string Filename { get; private set; }
            public SpriteFont SpriteFont => Assets.SpriteFont.Get(Filename);
            public static implicit operator SpriteFont(ManagedSpriteFont v) => v.SpriteFont;
            public ManagedSpriteFont(string filename) => Filename = filename;
        }

        public struct ManagedSoundEffect
        {
            public string Filename { get; private set; }
            private SoundEffectInstance _fxinstance;
            public SoundEffectInstance Instance
            {
                get
                {
                    SoundEffect.Get(Filename);
                    return _fxinstance;
                }
            }
            public static implicit operator SoundEffectInstance(ManagedSoundEffect v) => v.Instance;
            public ManagedSoundEffect(string filename)
            {
                Filename = filename;
                _fxinstance = SoundEffect.Get(Filename).CreateInstance();
            }
        }

        public struct ManagedSong
        {
            public string Filename { get; private set; }
            public Song Song => Assets.Song.Get(Filename);

            public static implicit operator Song(ManagedSong v) => v.Song;
            public ManagedSong(string filename) => Filename = filename;
        }

        private abstract class ControlledAsset<T> : IDynamic
            where T : class
        {
            protected T _asset { get; set; }
            public T Asset
            {
                get
                {
                    UnusedTime = TimeSpan.Zero;
                    return _asset;
                }
                set => _asset = value;
            }
            public string FileName { get; set; }
            public TimeSpan UnusedTime { get; set; }
            public ID ID { get; set; }

            static readonly TimeSpan LifeSpan = new TimeSpan(0, 1, 0);

            public void Update(GameTime gt)
            {
                UnusedTime += gt.ElapsedGameTime;
                if (UnusedTime >= LifeSpan)
                    Destroy();
            }

            public abstract void Destroy();

            public ControlledAsset(T asset, string filename)
            {
                FileName = filename;
                Asset = asset;
                UnusedTime = TimeSpan.Zero;
                Log.Verbose($"Creating new Managed Asset instance, holding asset of type {typeof(T).Name}");
            }

        }

        private class ControlledTexture2D : ControlledAsset<Texture2D>
        {
            public override void Destroy()
            {
                Texture2D.Unload(FileName);
            }
            public ControlledTexture2D(Texture2D asset, string filename) :
                base(asset, filename)
            { }
        }

        private class ControlledSpriteFont : ControlledAsset<SpriteFont>
        {
            public override void Destroy()
            {
                SpriteFont.Unload(FileName);
            }
            public ControlledSpriteFont(SpriteFont asset, string filename) :
                base(asset, filename)
            { }
        }

        private class ControlledSoundEffect : ControlledAsset<SoundEffect>
        {
            public override void Destroy()
            {
                SoundEffect.Unload(FileName);
            }
            public ControlledSoundEffect(SoundEffect asset, string filename) :
                base(asset, filename)
            { }
        }

        private class ControlledSong : ControlledAsset<Song>
        {
            public override void Destroy()
            {
                Song.Unload(FileName);
            }
            public ControlledSong(Song asset, string filename) :
                base(asset, filename)
            { }
        }

    }
}
