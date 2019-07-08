// Decompiled with JetBrains decompiler
// Type: xNet.RequestParams
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.Collections.Generic;

namespace xNet
{
  public class RequestParams : List<KeyValuePair<string, string>>
  {
    public object this[string paramName]
    {
      set
      {
        if (paramName == null)
          throw new ArgumentNullException(nameof (paramName));
        if (paramName.Length == 0)
          throw ExceptionHelper.EmptyString(nameof (paramName));
        string str = value == null ? string.Empty : value.ToString();
        this.Add(new KeyValuePair<string, string>(paramName, str));
      }
    }
  }
}
