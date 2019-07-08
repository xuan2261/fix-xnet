// Decompiled with JetBrains decompiler
// Type: xNet.FormUrlEncodedContent
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

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
