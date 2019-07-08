// Decompiled with JetBrains decompiler
// Type: xNet.DownloadProgressChangedEventArgs
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

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
