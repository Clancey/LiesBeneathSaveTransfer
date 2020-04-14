using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiesBeneathSaveTransfer
{
	public class BackupStream : Stream
	{
		Stream stream;
		public BackupStream(Stream fileStream)
		{
			stream = fileStream;
			var s = "\x1f\x8b\x08\x00\x00\x00\x00\x00";
			tarHeader = Encoding.UTF8.GetBytes(s);
		}
		public override bool CanRead => stream.CanRead;

		public override bool CanSeek => stream.CanSeek;

		public override bool CanWrite => stream.CanWrite;

		public override long Length => stream.Length;

		
		public override long Position { get => stream.Position; set => stream.Position = value; }

		public override void Flush()
		{
			stream.Flush();
		}
		byte[] tarHeader;
		public override int Read(byte[] buffer, int offset, int count)
		{
			var startPosition = Position;
			var read = stream.Read(buffer, offset, count);
			if (startPosition < 25)
			{
				//We need to overwrite the header;
				var headerLength = count + offset - 25;
				var newHEader = tarHeader.Skip((int)startPosition).Take(headerLength).ToArray();
				Array.Copy(newHEader, 0, buffer, offset, headerLength);
			}
			return read;
		}

		public override long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin);

		public override void SetLength(long value) => stream.SetLength(value);

		public override void Write(byte[] buffer, int offset, int count)
		{
			stream.Write(buffer, offset, count);
		}
	}
}
