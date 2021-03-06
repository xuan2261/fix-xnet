﻿// Decompiled with JetBrains decompiler
// Type: xNet.BytesContent
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

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
