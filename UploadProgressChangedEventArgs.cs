// Decompiled with JetBrains decompiler
// Type: xNet.UploadProgressChangedEventArgs
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

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
