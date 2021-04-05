using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching
{
    public class StreamLengthGuard : Stream
    {
        private readonly Stream _streamImplementation;
        private readonly string _domain;
        private readonly long _maxLength;

        public StreamLengthGuard(Stream streamImplementation, string domain, long maxLength)
        {
            _streamImplementation = streamImplementation;
            _domain = domain;
            _maxLength = maxLength;
        }

        public override void Flush()
        {
            _streamImplementation.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _streamImplementation.Read(buffer, offset, count);
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

        public override long Length => _maxLength;

        public override long Position
        {
            get => _streamImplementation.Position;
            set => _streamImplementation.Position = value;
        }

        private long _observedLength;
        
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            // ReSharper disable once CA1835
            var readAsync = await base.ReadAsync(buffer, offset, count, cancellationToken);
            _observedLength += _observedLength;

            if (_observedLength > _maxLength)
                throw new MaxImageLengthException(_domain, _maxLength);
            
            return readAsync;
        }

        protected override void Dispose(bool disposing)
        {
            _streamImplementation.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            return _streamImplementation.DisposeAsync();
        }
    }
}