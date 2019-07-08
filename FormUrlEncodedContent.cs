// Decompiled with JetBrains decompiler
// Type: xNet.FormUrlEncodedContent
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace xNet
{
  public class FormUrlEncodedContent : BytesContent
  {
    public FormUrlEncodedContent(
      IEnumerable<KeyValuePair<string, string>> content,
      bool dontEscape = false,
      Encoding encoding = null)
    {
      if (content == null)
        throw new ArgumentNullException(nameof (content));
      this._content = Encoding.ASCII.GetBytes(Http.ToPostQueryString(content, dontEscape, encoding));
      this._offset = 0;
      this._count = this._content.Length;
      this._contentType = "application/x-www-form-urlencoded";
    }
  }
}
