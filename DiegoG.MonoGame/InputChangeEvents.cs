using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiegoG.MonoGame.InputEvents
{
    public static class InputChanged
    {
        private static CancellationTokenSource CancellationTokenSource { get; set; }
        private static CancellationToken CancellationToken { get; set; }
        private static Task ServiceTask { get; set; }

#warning Won't this cause concurrency issues when the events are fired?
        public static void Start()
        {
            if (ServiceStarted)
                throw new InvalidOperationException("Cannot start InputChangeEvent Service since it is already started.");
            
            CancellationToken = CancellationTokenSource.Token;
            ServiceTask = Task.Run(
                async () =>
                {
                    while (true)
                    {
                        CancellationToken.ThrowIfCancellationRequested();
                        KeyboardState ks = Keyboard.GetState();
                        if (!(Keyboard_prevstate == ks))
                        {
                            KeyboardStateChanged(Keyboard_prevstate, ks);
                            Keyboard_prevstate = ks;
                        }
                        MouseState ms = Mouse.GetState();
                        if (!(Mouse_prevstate == ms))
                        {
                            MouseStateChanged(Mouse_prevstate, ms);
                            Mouse_prevstate = ms;
                        }
                        await Task.Delay(ServiceDelayFrame);
                    }
                }
            , CancellationToken);
            ServiceStarted = true;
        }
        public static void Stop()
        {
            if (!ServiceStarted)
                throw new InvalidOperationException("Cannot stop InputChangeEvent Service since it hasn't been started.");
            ServiceStarted = false;
            CancellationTokenSource.Cancel();
        }

        /// <summary>
        /// The amount of time the loop will wait before checking again, in milliseconds
        /// </summary>
        public static int ServiceDelayFrame { get; set; } = 10;
        public static TaskStatus ServiceStatus => ServiceTask.Status;
        public static AggregateException ServiceException => ServiceTask.Exception;
        public static bool ServiceIsFaulted => ServiceTask.IsFaulted;
        public static bool ServiceStarted { get; private set; } = false;
        public static KeyboardState Keyboard_prevstate { get; private set; } = Keyboard.GetState();
        public static MouseState Mouse_prevstate { get; private set; } = Mouse.GetState();

        public delegate void KeyboardInput(KeyboardState ksprev, KeyboardState ksnow);
        public static event KeyboardInput KeyboardStateChanged;

        public delegate void MouseInput(MouseState msprev, MouseState msnow);
        public static event MouseInput MouseStateChanged;
    }

}
