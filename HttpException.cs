// Decompiled with JetBrains decompiler
// Type: xNet.HttpException
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace xNet
{
  [Serializable]
  public sealed class HttpException : NetException
  {
    public HttpExceptionStatus Status { get; internal set; }

    public HttpStatusCode HttpStatusCode { get; private set; }

    internal bool EmptyMessageBody { get; set; }

    public HttpException()
      : this(Resources.HttpException_Default, (Exception) null)
    {
    }

    public HttpException(string message, Exception innerException = null)
      : base(message, innerException)
    {
    }

    public HttpException(
      string message,
      HttpExceptionStatus status,
      HttpStatusCode httpStatusCode = HttpStatusCode.None,
      Exception innerException = null)
      : base(message, innerException)
    {
      this.Status = status;
      this.HttpStatusCode = httpStatusCode;
    }

    protected HttpException(SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
      if (serializationInfo == null)
        return;
      this.Status = (HttpExceptionStatus) serializationInfo.GetInt32(nameof (Status));
      this.HttpStatusCode = (HttpStatusCode) serializationInfo.GetInt32(nameof (HttpStatusCode));
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public override void GetObjectData(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
    {
      base.GetObjectData(serializationInfo, streamingContext);
      if (serializationInfo == null)
        return;
      serializationInfo.AddValue("Status", (int) this.Status);
      serializationInfo.AddValue("HttpStatusCode", (int) this.HttpStatusCode);
    }
  }
}
