// Decompiled with JetBrains decompiler
// Type: xNet.CookieDictionary
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace xNet
{
  public class CookieDictionary : Dictionary<string, string>
  {
    public bool IsLocked { get; set; }

    public CookieDictionary(bool isLocked = false)
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      this.IsLocked = isLocked;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) this)
        stringBuilder.AppendFormat("{0}={1}; ", (object) keyValuePair.Key, (object) keyValuePair.Value);
      if (stringBuilder.Length > 0)
        stringBuilder.Remove(stringBuilder.Length - 2, 2);
      return stringBuilder.ToString();
    }
  }
}
