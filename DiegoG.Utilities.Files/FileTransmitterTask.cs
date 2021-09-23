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
    public class FileTransmitterTask : IDisposable, IAsyncDisposable
    {
        private readonly SemaphoreSlim AwaiterLock = new SemaphoreSlim(1, 1);

        private readonly IPAddress? Address;
        private readonly string? Host;
        private readonly ushort Port;
        private readonly TcpClient Tcp;
        private readonly FileCompressionStream CompressionStream;

        private bool disposedValue;
        private Task? TransmitTask;

        public FileTransmitterTask(string host, ushort port, string inputfile, int buffersize)
        {
            Tcp = new TcpClient() { SendBufferSize = buffersize };
            Host = host;
            Port = port;
            CompressionStream = new FileCompressionStream(inputfile, Tcp.GetStream(), buffersize);
        }

        public FileTransmitterTask(IPAddress address, ushort port, Stream input, int buffersize)
        {
            Tcp = new TcpClient() { SendBufferSize = buffersize };
            Address = address;
            Port = port;
            CompressionStream = new FileCompressionStream(input, Tcp.GetStream(), buffersize);
        }

        public async Task Transmit()
        {
            try
            {
                Task t;
                await AwaiterLock.WaitAsync();
                    t = (TransmitTask ??= Transmit_task());
                AwaiterLock.Release();
                await t;
            }
            finally
            {
                AwaiterLock.Release();
            }
        }

        protected virtual async Task Transmit_task()
        {
            byte[] response = new byte[Helper.NextBatchSignalLength];
            var stream = Tcp.GetStream();
            if (Address != null)
                await Tcp.ConnectAsync(Address, Port);
            else
                await Tcp.ConnectAsync(Host, Port);
            while (await CompressionStream.CompressNextAsync())
            {
                while (0 >= Tcp.Available)
                    await Task.Delay(100);
                await stream.ReadAsync(response.AsMemory(0, Helper.NextBatchSignalLength));
                if (!response.SequenceEqual(Helper.NextBatchSignal))
                    throw new InvalidDataException("The Receiver failed to confirm the next batch of data");
            }
        }

        public TaskAwaiter GetAwaiter()
        {
            try
            {
                Task t;
                AwaiterLock.Wait();
                t = (TransmitTask ??= Transmit_task());
                return t.GetAwaiter();
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
                    Tcp.Dispose();
                    CompressionStream.Dispose();
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
            await new ValueTask(Task.Run(Tcp.Dispose));
            await CompressionStream.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
