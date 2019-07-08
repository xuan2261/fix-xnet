// Decompiled with JetBrains decompiler
// Type: xNet.StringContent
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

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
