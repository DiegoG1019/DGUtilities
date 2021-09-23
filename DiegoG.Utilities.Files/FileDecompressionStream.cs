using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.IO.Files
{
    public class FileDecompressionStream : IDisposable, IAsyncDisposable
    {
        private readonly Stream Input;
        private readonly GZipStream Compress;
        private readonly byte[] Buffer;

        private bool disposedValue;

        public static Stream CreateInputFileStream(string file)
            => new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

        public static Stream CreateOutputFileStream(string file)
        {
            Directory.CreateDirectory(file);
            return new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public FileDecompressionStream(string inputfile, string outputfile, int buffersize)
            : this(CreateInputFileStream(inputfile), CreateOutputFileStream(outputfile), buffersize)
        { }

        public FileDecompressionStream(Stream input, string outputfile, int buffersize)
            : this(input, CreateOutputFileStream(outputfile), buffersize)
        { }

        public FileDecompressionStream(string inputfile, Stream output, int buffersize)
            : this(CreateInputFileStream(inputfile), output, buffersize)
        { }

        public FileDecompressionStream(Stream input, Stream output, int buffersize)
        {
            Input = input;
            Compress = new GZipStream(output, CompressionMode.Decompress, false);
            Buffer = new byte[buffersize];
        }

        public virtual Task<bool> DecompressNextAsync() => Task.Run(() =>
        {
            int read = Input.Read(Buffer, 0, Buffer.Length);
            Compress.Write(Buffer, 0, read);
            return read > 0;
        });

        public virtual async Task DecompressAsync()
        {
            while (await DecompressNextAsync()) ;
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
