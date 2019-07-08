// Decompiled with JetBrains decompiler
// Type: xNet.HttpContent
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System.IO;

namespace xNet
{
  public abstract class HttpContent
  {
    protected string _contentType = string.Empty;

    public string ContentType
    {
      get
      {
        return this._contentType;
      }
      set
      {
        this._contentType = value ?? string.Empty;
      }
    }

    public abstract long CalculateContentLength();

    public abstract void WriteTo(Stream stream);

    public void Dispose()
    {
      this.Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
    }
  }
}
