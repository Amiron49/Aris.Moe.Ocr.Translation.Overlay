using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Aris.Moe.Ocr
{
    public class StreamProgressTracker : Stream
    {
        private readonly Stream _streamImplementation;
        private readonly IProgress<double> _progress;
        private readonly long _size;

        public StreamProgressTracker(Stream streamImplementation, IProgress<double> progress, long size)
        {
            _streamImplementation = streamImplementation;
            _progress = progress;
            _size = size;
        }

        public override void Flush()
        {
            _streamImplementation.Flush();
            _progress.Report(1);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _streamImplementation.Read(buffer, offset, count);
        }

        private long _alreadyLoadedBytes;
        
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var readAsync = await base.ReadAsync(buffer, offset, count, cancellationToken);

            _alreadyLoadedBytes += readAsync;

            var progress = (double) _alreadyLoadedBytes / _size;

            _progress.Report(progress);

            return readAsync;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _streamImplementation.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _streamImplementation.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _streamImplementation.Write(buffer, offset, count);
        }

        public override bool CanRead => _streamImplementation.CanRead;

        public override bool CanSeek => _streamImplementation.CanSeek;

        public override bool CanWrite => _streamImplementation.CanWrite;

        public override long Length => _streamImplementation.Length;

        public override long Position
        {
            get => _streamImplementation.Position;
            set => _streamImplementation.Position = value;
        }

        protected override void Dispose(bool disposing)
        {
            _streamImplementation.Dispose();
        }
    }
}