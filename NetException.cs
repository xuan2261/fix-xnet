﻿// Decompiled with JetBrains decompiler
// Type: xNet.NetException
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

using System;
using System.Runtime.Serialization;

namespace xNet
{
  [Serializable]
  public class NetException : Exception
  {
    public NetException()
      : this(Resources.NetException_Default, (Exception) null)
    {
    }

    public NetException(string message, Exception innerException = null)
      : base(message, innerException)
    {
    }

    protected NetException(SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
