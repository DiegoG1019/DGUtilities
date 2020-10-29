using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.IO;

namespace DiegoG.MonoGame
{
    public static partial class Assets
    {
        private abstract class Manager<T, TMA> where T : ControlledAsset<TMA>, IDynamic where TMA : class
        {
            public Loaded.LoadedList<ControlledAsset<TMA>> LoadedAssets = new Loaded.LoadedList<ControlledAsset<TMA>>("LoadedAssets");
            Dictionary<string, ID> IDs = new Dictionary<string, ID>();

            protected abstract ControlledAsset<TMA> CreateManagedAsset(string filename);
            public abstract string BaseDirectory { get; }

            public void Update(GameTime gt)
            {
                foreach (ID id in LoadedAssets)
                {
                    LoadedAssets[id].Update(gt);
                }
            }

            public TMA Get(string filename)
            {
                if (IDs.ContainsKey(filename))
                {
                    goto ReturnAsset;
                }
                ControlledAsset<TMA> newmanagedasset = CreateManagedAsset(Path.Combine(BaseDirectory, filename)); //new ManagedAsset<T>(Program.Game.Content.Load<T>(filename), filename);
                newmanagedasset.SetID(LoadedAssets.Add(newmanagedasset));
                IDs.Add(filename, newmanagedasset.ID);

                ReturnAsset:;
                return LoadedAssets[IDs[filename]].Asset;
            }

            public void Unload(string filename)
            {
                if (IDs.ContainsKey(filename))
                {
                    LoadedAssets.Remove(IDs[filename]);
                    IDs.Remove(filename);
                }
            }

        }
        private class Texture2DManager : Manager<ControlledTexture2D, Texture2D>
        {
            const string _basedir = "Textures";
            public override string BaseDirectory
            {
                get
                {
                    return _basedir;
                }
            }
            protected override ControlledAsset<Texture2D> CreateManagedAsset(string filename)
            {
                return new ControlledTexture2D(Program.GameObject.Content.Load<Texture2D>(filename), filename);
            }
        }

        private class SpriteFontManager : Manager<ControlledSpriteFont, SpriteFont>
        {
            const string _basedir = "Fonts";
            public override string BaseDirectory
            {
                get
                {
                    return _basedir;
                }
            }
            protected override ControlledAsset<SpriteFont> CreateManagedAsset(string filename)
            {
                return new ControlledSpriteFont(Program.GameObject.Content.Load<SpriteFont>(filename), filename);
            }
        }

        private class SongManager : Manager<ControlledSong, Song>
        {
            const string _basedir = "Songs";
            public override string BaseDirectory
            {
                get
                {
                    return _basedir;
                }
            }
            protected override ControlledAsset<Song> CreateManagedAsset(string filename)
            {
                return new ControlledSong(Program.GameObject.Content.Load<Song>(filename), filename);
            }
        }

        private class SoundEffectManager : Manager<ControlledSoundEffect, SoundEffect>
        {
            const string _basedir = "SoundEffects";
            public override string BaseDirectory
            {
                get
                {
                    return _basedir;
                }
            }
            protected override ControlledAsset<SoundEffect> CreateManagedAsset(string filename)
            {
                return new ControlledSoundEffect(Program.GameObject.Content.Load<SoundEffect>(filename), filename);
            }
        }

    }

}
