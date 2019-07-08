// Decompiled with JetBrains decompiler
// Type: xNet.ProxyException
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;
using System.Runtime.Serialization;

namespace xNet
{
  [Serializable]
  public sealed class ProxyException : NetException
  {
    public ProxyClient ProxyClient { get; private set; }

    public ProxyException()
      : this(Resources.ProxyException_Default)
    {
    }

    public ProxyException(string message, Exception innerException = null)
      : base(message, innerException)
    {
    }

    public ProxyException(string message, ProxyClient proxyClient, Exception innerException = null)
      : base(message, innerException)
    {
      this.ProxyClient = proxyClient;
    }

    protected ProxyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }

    public ProxyException(string message)
      : base(message)
    {
    }
  }
}
