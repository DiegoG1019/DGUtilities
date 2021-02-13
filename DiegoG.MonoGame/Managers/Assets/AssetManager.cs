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
        public static Game Game { get; set; }
        private abstract class Manager<T, TMA> where T : ControlledAsset<TMA>, IDynamic where TMA : class
        {
            public LoadedLists.LoadedList<ControlledAsset<TMA>> LoadedAssets = new LoadedLists.LoadedList<ControlledAsset<TMA>>("LoadedAssets", Game);
            readonly Dictionary<string, ID> IDs = new Dictionary<string, ID>();

            protected abstract ControlledAsset<TMA> CreateManagedAsset(string filename);
            public abstract string BaseDirectory { get; }

            public TMA Get(string filename)
                => GetControlled(filename).Asset;
            public ControlledAsset<TMA> GetControlled(string filename)
            {
                if (IDs.ContainsKey(filename))
                {
                    goto ReturnAsset;
                }

                ControlledAsset<TMA> newmanagedasset = CreateManagedAsset(Path.Combine(BaseDirectory, filename)); //new ManagedAsset<T>(Program.Game.Content.Load<T>(filename), filename);
                LoadedAssets.Add(newmanagedasset);
                IDs.Add(filename, newmanagedasset.ID);

            ReturnAsset:;
                return LoadedAssets[IDs[filename]];
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
            public override string BaseDirectory => _basedir;
            protected override ControlledAsset<Texture2D> CreateManagedAsset(string filename) => new ControlledTexture2D(Game.Content.Load<Texture2D>(filename), filename);
        }

        private class SpriteFontManager : Manager<ControlledSpriteFont, SpriteFont>
        {
            const string _basedir = "Fonts";
            public override string BaseDirectory => _basedir;
            protected override ControlledAsset<SpriteFont> CreateManagedAsset(string filename) => new ControlledSpriteFont(Game.Content.Load<SpriteFont>(filename), filename);
        }

        private class SongManager : Manager<ControlledSong, Song>
        {
            const string _basedir = "Songs";
            public override string BaseDirectory => _basedir;
            protected override ControlledAsset<Song> CreateManagedAsset(string filename) => new ControlledSong(Game.Content.Load<Song>(filename), filename);
        }

        private class SoundEffectManager : Manager<ControlledSoundEffect, SoundEffect>
        {
            const string _basedir = "SoundEffects";
            public override string BaseDirectory => _basedir;
            protected override ControlledAsset<SoundEffect> CreateManagedAsset(string filename) => new ControlledSoundEffect(Game.Content.Load<SoundEffect>(filename), filename);
        }

    }

}
