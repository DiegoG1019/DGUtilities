using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DiegoG.Utilities.IO.Files
{
    public class FileReceiverTask : IDisposable, IAsyncDisposable
    {
        private readonly SemaphoreSlim AwaiterLock = new SemaphoreSlim(1, 1);

        private readonly TcpListener Tcp;
        private readonly int Buffersize;
        private readonly Stream Output;
        
        private FileDecompressionStream? DecompressionStream;
        private TcpClient? ConnectedClient;

        private bool IsConnected;
        private bool disposedValue;
        private Task? TransmitTask;

        public FileReceiverTask(IPAddress local, ushort port, string outputfile, int buffersize)
        {
            Tcp = new TcpListener(local, port) { ExclusiveAddressUse = true };
            Output = FileDecompressionStream.CreateOutputFileStream(outputfile);
            Buffersize = buffersize;
        }

        public FileReceiverTask(IPAddress local, ushort port, Stream output, int buffersize)
        {
            Tcp = new TcpListener(local, port) { ExclusiveAddressUse = true };
            Output = output;
            Buffersize = buffersize;
        }

        /// <summary>
        /// Waits for a connection to succeed. If there are no pending connections and timeout is set, it will wait for one to appear
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns>True if a connection was established, false otherwise</returns>
        public async Task<bool> WaitForConnection(TimeSpan? timeout = null)
        {
            try
            {
                await AwaiterLock.WaitAsync();
                if (ConnectedClient != null)
                    throw new InvalidOperationException("Only one connection allowed");
            }
            finally
            {
                AwaiterLock.Release();
            }

            if (Tcp.Pending())
                return await TryConnect() ? true : throw new Exception("There was a problem connecting to the Transmitter");

            if (timeout != null)
            {
                await Task.Delay(timeout.Value);
                return await TryConnect();
            }

            return false;

            async Task<bool> TryConnect()
            {
                ConnectedClient = await Tcp.AcceptTcpClientAsync();
                DecompressionStream = new FileDecompressionStream(ConnectedClient.GetStream(), Output, Buffersize);
                IsConnected = true;
                return true;
            }
        }

        public void Disconnect()
        {
            if (ConnectedClient != null)
            {
                ConnectedClient.Dispose();
                ConnectedClient = null;
            }
        }

        public async Task Receive()
        {
            try
            {
                Task t;
                await AwaiterLock.WaitAsync();
                t = (TransmitTask ??= Receive_task());
                AwaiterLock.Release();
                await t;
            }
            finally
            {
                AwaiterLock.Release();
            }
        }

        protected virtual async Task Receive_task()
        {
            await ThrowIfNotConnectedAsync();
            var response = Helper.NextBatchSignal.ToArray().AsMemory(0, Helper.NextBatchSignalLength);

            var stream = ConnectedClient!.GetStream();
            while (await DecompressionStream!.DecompressNextAsync())
                await stream.WriteAsync(response);
        }

        public TaskAwaiter GetAwaiter()
        {
            try
            {
                Task t;
                AwaiterLock.Wait();
                t = (TransmitTask ??= Receive_task());
                return t.GetAwaiter();
            }
            finally
            {
                AwaiterLock.Release();
            }
        }

        private async Task ThrowIfNotConnectedAsync()
        {
            try
            {
                await AwaiterLock.WaitAsync();
                if (IsConnected is false)
                    throw new InvalidOperationException("Cannot begin receiving when there are no connections");
            }
            finally
            {
                AwaiterLock.Release();
            }
        }

        private void ThrowIfNotConnected()
        {
            try
            {
                AwaiterLock.Wait();
                if (IsConnected is false)
                    throw new InvalidOperationException("Cannot begin receiving when there are no connections");
            }
            finally
            {
                AwaiterLock.Release();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Tcp.Stop();
                    DecompressionStream?.Dispose();
                    ConnectedClient?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await new ValueTask(Task.Run(Tcp.Stop));
            await (DecompressionStream?.DisposeAsync() ?? new ValueTask(Task.CompletedTask));
            if (ConnectedClient != null)
                await new ValueTask(Task.Run(ConnectedClient.Dispose));
            GC.SuppressFinalize(this);
        }
    }
}
