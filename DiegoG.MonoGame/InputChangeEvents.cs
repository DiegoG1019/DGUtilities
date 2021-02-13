using DiegoG.Utilities.Collections;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DiegoG.MonoGame.InputEvents
{
    public static class InputChanged
    {
        private static CancellationTokenSource CancellationTokenSource { get; set; }
        private static CancellationToken CancellationToken { get; set; }
        private static Task ServiceTask { get; set; }
        private static LimitedMemberQueue<(KeyboardState ksprev, KeyboardState ksnow)> KeyboardInputQueue { get; } = new();
        private static LimitedMemberQueue<(MouseState msprev, MouseState msnow)> MouseInputQueue { get; } = new();
        private static ConcurrentQueue<(KeyboardState ksprev, KeyboardState ksnow)> KeyboardInputImmediateQueue { get; } = new();
        private static ConcurrentQueue<(MouseState msprev, MouseState msnow)> MouseInputImmediateQueue { get; } = new();

        private static (bool IsFaulted, Exception ex) Fault
        {
            get
            {
                lock (FaultSyncKey)
                {
                    return FaultField;
                }
            }
            set
            {
                lock (FaultSyncKey)
                {
                    FaultField = value;
                }
            }
        }
        private static (bool IsFaulted, Exception ex) FaultField = (false, null);
        private static readonly object FaultSyncKey = new();

        private static void EnqueueKeyboardInput(KeyboardState ksprev, KeyboardState ksnow)
        {
            KeyboardInputQueue.ForceEnqueue((ksprev, ksnow));
            KeyboardInputImmediateQueue.Enqueue((ksprev, ksnow));
        }
        private static void EnqueueMouseInput(MouseState msprev, MouseState msnow)
        {
            MouseInputQueue.ForceEnqueue((msprev, msnow));
            MouseInputImmediateQueue.Enqueue((msprev, msnow));
        }

        public static void Start()
        {
            if (ServiceStarted)
            {
                throw new InvalidOperationException("Cannot start InputChangeEvent Service since it is already started.");
            }

            CancellationToken = CancellationTokenSource.Token;
            ServiceTask = Task.Factory.StartNew(
                async () =>
                {
                    while (true)
                    {
                        CancellationToken.ThrowIfCancellationRequested();
                        if (Fault.IsFaulted)
                        {
                            throw Fault.ex;
                        }

                        KeyboardState ks = Keyboard.GetState();
                        if (!(Keyboard_prevstate == ks))
                        {
                            EnqueueKeyboardInput(Keyboard_prevstate, ks);
                            Keyboard_prevstate = ks;
                        }
                        MouseState ms = Mouse.GetState();
                        if (!(Mouse_prevstate == ms))
                        {
                            EnqueueMouseInput(Mouse_prevstate, ms);
                            Mouse_prevstate = ms;
                        }
                        await Task.Delay(ServiceDelayFrame);
                    }
                }
            , CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        while (true)
                        {
                            CancellationToken.ThrowIfCancellationRequested();
                            while (!KeyboardInputImmediateQueue.IsEmpty || !MouseInputImmediateQueue.IsEmpty)
                            {
                                if (KeyboardInputImmediateQueue.TryDequeue(out var ksin))
                                {
                                    KeyboardStateChangedImmediate?.Invoke(ksin.ksprev, ksin.ksnow);
                                }

                                if (MouseInputImmediateQueue.TryDequeue(out var msin))
                                {
                                    MouseStateChangedImmediate?.Invoke(msin.msprev, msin.msnow);
                                }
                            }
                            await Task.Delay(ServiceDelayFrame);
                        }
                    }
                    catch (Exception e)
                    {
                        Fault = (true, e);
                        throw;
                    }
                }
            , CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            ServiceStarted = true;
        }

        public static void Stop()
        {
            if (!ServiceStarted)
            {
                throw new InvalidOperationException("Cannot stop InputChangeEvent Service since it hasn't been started.");
            }

            ServiceStarted = false;
            CancellationTokenSource.Cancel();
            Fault = (false, null);
        }

        /// <summary>
        /// The amount of time the loop will wait before checking again. Defaults to 10ms
        /// </summary>
        public static TimeSpan ServiceDelayFrame { get; set; } = TimeSpan.FromMilliseconds(10);
        public static TaskStatus ServiceStatus => ServiceTask.Status;
        public static AggregateException ServiceException => ServiceTask.Exception;
        public static bool ServiceIsFaulted => ServiceTask.IsFaulted;
        public static bool ServiceStarted { get; private set; } = false;
        public static KeyboardState Keyboard_prevstate { get; private set; } = Keyboard.GetState();
        public static MouseState Mouse_prevstate { get; private set; } = Mouse.GetState();

        /// <summary>
        /// The number of inputs that will be buffered in the input queue. Set to -1 to remove limit
        /// </summary>
        public static int Mouse_InputBuffer
        {
            get => MouseInputQueue.Capacity;
            set => MouseInputQueue.Capacity = value;
        }

        /// <summary>
        /// The number of inputs that will be buffered in the input queue. Set to -1 to remove limit
        /// </summary>
        public static int Keyboard_InputBuffer
        {
            get => KeyboardInputQueue.Capacity;
            set => KeyboardInputQueue.Capacity = value;
        }

        public delegate void KeyboardInput(KeyboardState ksprev, KeyboardState ksnow);
        public delegate void MouseInput(MouseState msprev, MouseState msnow);

        /// <summary>
        /// This event is fired in a given thread through NextInput() and CheckAllInput() methods. Using both this and immediate variations may result in duplicate inputs
        /// </summary>
        public static event KeyboardInput KeyboardStateChanged;

        /// <summary>
        /// This event is fired in a given thread through NextInput() and CheckAllInput() methods. Using both this and immediate variations may result in duplicate inputs
        /// </summary>
        public static event MouseInput MouseStateChanged;

        /// <summary>
        /// This event is fired at the moment an input is received, without needing another thread to check for results. This may result in concurrency issues.
        /// This event is fired from a different thread than the one used to check inputs. Using both this and same-thread variations may result in duplicate inputs
        /// </summary>
        public static event KeyboardInput KeyboardStateChangedImmediate;

        /// <summary>
        /// This event is fired at the moment an input is received, without needing another thread to check for results. This may result in concurrency issues.
        /// This event is fired from a different thread than the one used to check inputs. Using both this and same-thread variations may result in duplicate inputs
        /// </summary>
        public static event MouseInput MouseStateChangedImmediate;

        /// <summary>
        /// Checks the input queue, and fires input events accordingly for the next queued input. Use this for same-thread Input events.
        /// </summary>
        public static void NextInput()
        {
            if (ServiceIsFaulted)
            {
                throw ServiceException;
            }

            if (MouseInputQueue.TryDequeue(out var msinput))
            {
                MouseStateChanged?.Invoke(msinput.msprev, msinput.msnow);
            }

            if (KeyboardInputQueue.TryDequeue(out var ksinput))
            {
                KeyboardStateChanged?.Invoke(ksinput.ksprev, ksinput.ksnow);
            }
        }
        /// <summary>
        /// Checks the input queue, and fires input events accordingly for all queued inputs. Use this for same-thread Input events
        /// </summary>
        public static void CheckAllInput()
        {
            while (!MouseInputQueue.IsEmpty || !KeyboardInputQueue.IsEmpty)
            {
                NextInput();
            }
        }
    }
}
