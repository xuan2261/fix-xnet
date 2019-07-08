// Decompiled with JetBrains decompiler
// Type: xNet.StreamContent
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

using System;
using System.IO;

namespace xNet
{
  public class StreamContent : HttpContent
  {
    protected Stream _content;
    protected int _bufferSize;
    protected long _initialStreamPosition;

    public StreamContent(Stream content, int bufferSize = 32768)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      if (!content.CanRead || !content.CanSeek)
        throw new ArgumentException(Resources.ArgumentException_CanNotReadOrSeek, nameof (content));
      if (bufferSize < 1)
        throw ExceptionHelper.CanNotBeLess<int>(nameof (bufferSize), 1);
      this._content = content;
      this._bufferSize = bufferSize;
      this._initialStreamPosition = this._content.Position;
      this._contentType = "application/octet-stream";
    }

    protected StreamContent()
    {
    }

    public override long CalculateContentLength()
    {
      this.ThrowIfDisposed();
      return this._content.Length;
    }

    public override void WriteTo(Stream stream)
    {
      this.ThrowIfDisposed();
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      this._content.Position = this._initialStreamPosition;
      byte[] buffer = new byte[this._bufferSize];
      while (true)
      {
        int count = this._content.Read(buffer, 0, buffer.Length);
        if (count != 0)
          stream.Write(buffer, 0, count);
        else
          break;
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing || this._content == null)
        return;
      this._content.Dispose();
      this._content = (Stream) null;
    }

    private void ThrowIfDisposed()
    {
      if (this._content == null)
        throw new ObjectDisposedException(nameof (StreamContent));
    }
  }
}
