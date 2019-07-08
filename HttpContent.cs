// Decompiled with JetBrains decompiler
// Type: xNet.HttpContent
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

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
