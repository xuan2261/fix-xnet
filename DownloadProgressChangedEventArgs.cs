// Decompiled with JetBrains decompiler
// Type: xNet.DownloadProgressChangedEventArgs
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;

namespace xNet
{
  public sealed class DownloadProgressChangedEventArgs : EventArgs
  {
    public long BytesReceived { get; private set; }

    public long TotalBytesToReceive { get; private set; }

    public double ProgressPercentage
    {
      get
      {
        return (double) this.BytesReceived / (double) this.TotalBytesToReceive * 100.0;
      }
    }

    public DownloadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive)
    {
      this.BytesReceived = bytesReceived;
      this.TotalBytesToReceive = totalBytesToReceive;
    }
  }
}
