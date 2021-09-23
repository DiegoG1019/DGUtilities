using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace DiegoG.Utilities.IO.Files
{
    public class FileCompressionStream : IDisposable, IAsyncDisposable
    {
        private readonly Stream Input;
        private readonly GZipStream Compress;
        private readonly byte[] Buffer;

        private bool disposedValue;

        private static Stream CreateInputFileStream(string file)
            => new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

        private static Stream CreateOutputFileStream(string file)
        {
            Directory.CreateDirectory(file);
            return new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public FileCompressionStream(string inputfile, string outputfile, int buffersize)
            : this(CreateInputFileStream(inputfile), CreateOutputFileStream(outputfile), buffersize)
        { }

        public FileCompressionStream(Stream input, string outputfile, int buffersize)
            : this(input, CreateOutputFileStream(outputfile), buffersize)
        { }

        public FileCompressionStream(string inputfile, Stream output, int buffersize)
            : this(CreateInputFileStream(inputfile), output, buffersize)
        { }

        public FileCompressionStream(Stream input, Stream output, int buffersize)
        {
            Input = input;
            Compress = new GZipStream(output, CompressionMode.Compress, false);
            Buffer = new byte[buffersize];
        }

        public virtual Task<bool> CompressNextAsync() => Task.Run(() =>
        {
            int read = Input.Read(Buffer, 0, Buffer.Length);
            Compress.Write(Buffer, 0, read);
            return read > 0;
        });

        public virtual async Task CompressAsync()
        {
            while (await CompressNextAsync()) ;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Compress.Dispose();
                    Input.Dispose();
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
            await Input.DisposeAsync();
            await Compress.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
