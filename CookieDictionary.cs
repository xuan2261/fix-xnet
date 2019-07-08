// Decompiled with JetBrains decompiler
// Type: xNet.CookieDictionary
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace xNet
{
  [Serializable]
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

    protected CookieDictionary(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
    {
      throw new NotImplementedException();
    }
  }
}
