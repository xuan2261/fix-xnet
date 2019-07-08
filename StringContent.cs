// Decompiled with JetBrains decompiler
// Type: xNet.StringContent
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.Text;

namespace xNet
{
  public class StringContent : BytesContent
  {
    public StringContent(string content)
      : this(content, Encoding.UTF8)
    {
    }

    public StringContent(string content, Encoding encoding)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      if (encoding == null)
        throw new ArgumentNullException(nameof (encoding));
      this._content = encoding.GetBytes(content);
      this._offset = 0;
      this._count = this._content.Length;
      this._contentType = "text/plain";
    }
  }
}
