// Decompiled with JetBrains decompiler
// Type: xNet.UploadProgressChangedEventArgs
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;

namespace xNet
{
  public sealed class UploadProgressChangedEventArgs : EventArgs
  {
    public long BytesSent { get; private set; }

    public long TotalBytesToSend { get; private set; }

    public double ProgressPercentage
    {
      get
      {
        return (double) this.BytesSent / (double) this.TotalBytesToSend * 100.0;
      }
    }

    public UploadProgressChangedEventArgs(long bytesSent, long totalBytesToSend)
    {
      this.BytesSent = bytesSent;
      this.TotalBytesToSend = totalBytesToSend;
    }
  }
}
