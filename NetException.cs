// Decompiled with JetBrains decompiler
// Type: xNet.NetException
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.Runtime.Serialization;

namespace xNet
{
  [Serializable]
  public class NetException : Exception
  {
    public NetException()
      : this(Resources.NetException_Default)
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

    public NetException(string message)
      : base(message)
    {
    }
  }
}
