using DiegoG.Utilities;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Serilog;
using System;
using System.Timers;

namespace DiegoG.MonoGame
{
    public static partial class Assets
    {
        /// <summary>
        /// Given in milliseconds, defaults to 5m (5 * 60 000)
        /// </summary>
        public static double DefaultLifeSpan { get; set; } = 5 * 60000;

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

        public abstract class ControlledAsset<T> : IDynamic
            where T : class
        {
            protected T _asset { get; set; }
            public T Asset
            {
                get
                {
                    Life.Reset();
                    return _asset;
                }
                protected internal set => _asset = value;
            }
            public string FileName { get; protected internal set; }
            public ID ID
            {
                get => IDField;
                set { IDField = value; IDChanged?.Invoke(this, new GenericEventArgs<ID>(value)); }
            }
            ID IDField;

            private Timer Life { get; init; }
            static Timer LifeSpan => new(DefaultLifeSpan);

            public event EventHandler<EventArgs> IDChanged;

            public virtual void Destroy()
            {
                Life.Elapsed -= Life_Elapsed;
                Life.Dispose();
            }

            protected internal ControlledAsset(T asset, string filename)
            {
                Life = LifeSpan;
                Life.Elapsed += Life_Elapsed;
                FileName = filename;
                Asset = asset;
                Log.Verbose($"Creating new Managed Asset instance, holding asset of type {typeof(T).Name}");
                Life.Start();
            }

            private void Life_Elapsed(object sender, ElapsedEventArgs e) => Destroy();

            public void Initialize() => throw new NotSupportedException();
        }

        public class ControlledTexture2D : ControlledAsset<Texture2D>
        {
            public override void Destroy()
            {
                Texture2D.Unload(FileName);
                base.Destroy();
            }

            protected internal ControlledTexture2D(Texture2D asset, string filename) :
                base(asset, filename)
            { }
        }

        public class ControlledSpriteFont : ControlledAsset<SpriteFont>
        {
            public override void Destroy()
            {
                SpriteFont.Unload(FileName);
                base.Destroy();
            }

            protected internal ControlledSpriteFont(SpriteFont asset, string filename) :
                base(asset, filename)
            { }
        }

        public class ControlledSoundEffect : ControlledAsset<SoundEffect>
        {
            public override void Destroy()
            {
                SoundEffect.Unload(FileName);
                base.Destroy();
            }
            protected internal ControlledSoundEffect(SoundEffect asset, string filename) :
                base(asset, filename)
            { }
        }

        public class ControlledSong : ControlledAsset<Song>
        {
            public override void Destroy()
            {
                Song.Unload(FileName);
                base.Destroy();
            }
            protected internal ControlledSong(Song asset, string filename) :
                base(asset, filename)
            { }
        }

    }
}
