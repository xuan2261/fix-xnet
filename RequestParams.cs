// Decompiled with JetBrains decompiler
// Type: xNet.RequestParams
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

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
