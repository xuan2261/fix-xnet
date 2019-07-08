// Decompiled with JetBrains decompiler
// Type: xNet.BytesContent
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.IO;

namespace xNet
{
  public class BytesContent : HttpContent
  {
    protected byte[] _content;
    protected int _offset;
    protected int _count;

    public BytesContent(byte[] content)
      : this(content, 0, content.Length)
    {
    }

    public BytesContent(byte[] content, int offset, int count)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      if (offset < 0)
        throw ExceptionHelper.CanNotBeLess<int>(nameof (offset), 0);
      if (offset > content.Length)
        throw ExceptionHelper.CanNotBeGreater<int>(nameof (offset), content.Length);
      if (count < 0)
        throw ExceptionHelper.CanNotBeLess<int>(nameof (count), 0);
      if (count > content.Length - offset)
        throw ExceptionHelper.CanNotBeGreater<int>(nameof (count), content.Length - offset);
      this._content = content;
      this._offset = offset;
      this._count = count;
      this._contentType = "application/octet-stream";
    }

    protected BytesContent()
    {
    }

    public override long CalculateContentLength()
    {
      return (long) this._content.Length;
    }

    public override void WriteTo(Stream stream)
    {
      stream.Write(this._content, this._offset, this._count);
    }
  }
}
