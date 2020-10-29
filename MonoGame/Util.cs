using static MagicGame.Program;

namespace DiegoG.MonoGame
{
    public static class Util
    {
        public static void SetWindowSize(int width, int height, bool isFullScreen = false, bool vsync = true, bool multisample = false)
        {
            GameObject.GraphicsManager.PreferredBackBufferWidth = width;
            GameObject.GraphicsManager.PreferredBackBufferHeight = height;
            GameObject.GraphicsManager.IsFullScreen = isFullScreen;
            GameObject.GraphicsManager.SynchronizeWithVerticalRetrace = vsync;
            GameObject.GraphicsManager.PreferMultiSampling = multisample;
            GameObject.GraphicsManager.ApplyChanges();
        }
    }
}
