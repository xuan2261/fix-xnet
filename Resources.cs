// Decompiled with JetBrains decompiler
// Type: xNet.Resources
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace xNet
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (xNet.Resources.resourceMan == null)
          xNet.Resources.resourceMan = new ResourceManager("xNet.Resources", typeof (xNet.Resources).Assembly);
        return xNet.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return xNet.Resources.resourceCulture;
      }
      set
      {
        xNet.Resources.resourceCulture = value;
      }
    }

    internal static string ArgumentException_CanNotReadOrSeek
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ArgumentException_CanNotReadOrSeek), xNet.Resources.resourceCulture);
      }
    }

    internal static string ArgumentException_EmptyString
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ArgumentException_EmptyString), xNet.Resources.resourceCulture);
      }
    }

    internal static string ArgumentException_HttpRequest_SetNotAvailableHeader
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ArgumentException_HttpRequest_SetNotAvailableHeader), xNet.Resources.resourceCulture);
      }
    }

    internal static string ArgumentException_MultiThreading_BegIndexRangeMoreEndIndex
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ArgumentException_MultiThreading_BegIndexRangeMoreEndIndex), xNet.Resources.resourceCulture);
      }
    }

    internal static string ArgumentException_OnlyAbsoluteUri
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ArgumentException_OnlyAbsoluteUri), xNet.Resources.resourceCulture);
      }
    }

    internal static string ArgumentException_WrongPath
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ArgumentException_WrongPath), xNet.Resources.resourceCulture);
      }
    }

    internal static string ArgumentOutOfRangeException_CanNotBeGreater
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_CanNotBeGreater), xNet.Resources.resourceCulture);
      }
    }

    internal static string ArgumentOutOfRangeException_CanNotBeLess
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_CanNotBeLess), xNet.Resources.resourceCulture);
      }
    }

    internal static string ArgumentOutOfRangeException_CanNotBeLessOrGreater
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_CanNotBeLessOrGreater), xNet.Resources.resourceCulture);
      }
    }

    internal static string ArgumentOutOfRangeException_StringHelper_MoreLengthString
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_StringHelper_MoreLengthString), xNet.Resources.resourceCulture);
      }
    }

    internal static string ArgumentOutOfRangeException_StringLengthCanNotBeMore
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_StringLengthCanNotBeMore), xNet.Resources.resourceCulture);
      }
    }

    internal static string DirectoryNotFoundException_DirectoryNotFound
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (DirectoryNotFoundException_DirectoryNotFound), xNet.Resources.resourceCulture);
      }
    }

    internal static string FormatException_ProxyClient_WrongPort
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (FormatException_ProxyClient_WrongPort), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_ClientError
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_ClientError), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_ConnectTimeout
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_ConnectTimeout), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_Default
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_Default), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_FailedConnect
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_FailedConnect), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_FailedGetHostAddresses
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_FailedGetHostAddresses), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_FailedReceiveMessageBody
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_FailedReceiveMessageBody), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_FailedReceiveResponse
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_FailedReceiveResponse), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_FailedSendRequest
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_FailedSendRequest), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_FailedSslConnect
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_FailedSslConnect), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_LimitRedirections
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_LimitRedirections), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_ReceivedEmptyResponse
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_ReceivedEmptyResponse), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_ReceivedWrongResponse
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_ReceivedWrongResponse), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_SeverError
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_SeverError), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_WaitDataTimeout
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_WaitDataTimeout), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_WrongChunkedBlockLength
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_WrongChunkedBlockLength), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_WrongCookie
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_WrongCookie), xNet.Resources.resourceCulture);
      }
    }

    internal static string HttpException_WrongHeader
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (HttpException_WrongHeader), xNet.Resources.resourceCulture);
      }
    }

    internal static string InvalidOperationException_ChainProxyClient_NotProxies
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ChainProxyClient_NotProxies), xNet.Resources.resourceCulture);
      }
    }

    internal static string InvalidOperationException_HttpResponse_HasError
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_HttpResponse_HasError), xNet.Resources.resourceCulture);
      }
    }

    internal static string InvalidOperationException_NotSupportedEncodingFormat
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_NotSupportedEncodingFormat), xNet.Resources.resourceCulture);
      }
    }

    internal static string InvalidOperationException_ProxyClient_WrongHost
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ProxyClient_WrongHost), xNet.Resources.resourceCulture);
      }
    }

    internal static string InvalidOperationException_ProxyClient_WrongPassword
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ProxyClient_WrongPassword), xNet.Resources.resourceCulture);
      }
    }

    internal static string InvalidOperationException_ProxyClient_WrongPort
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ProxyClient_WrongPort), xNet.Resources.resourceCulture);
      }
    }

    internal static string InvalidOperationException_ProxyClient_WrongUsername
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ProxyClient_WrongUsername), xNet.Resources.resourceCulture);
      }
    }

    internal static string NetException_Default
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (NetException_Default), xNet.Resources.resourceCulture);
      }
    }

    internal static string ProxyException_CommandError
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ProxyException_CommandError), xNet.Resources.resourceCulture);
      }
    }

    internal static string ProxyException_ConnectTimeout
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ProxyException_ConnectTimeout), xNet.Resources.resourceCulture);
      }
    }

    internal static string ProxyException_Default
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ProxyException_Default), xNet.Resources.resourceCulture);
      }
    }

    internal static string ProxyException_Error
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ProxyException_Error), xNet.Resources.resourceCulture);
      }
    }

    internal static string ProxyException_FailedConnect
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ProxyException_FailedConnect), xNet.Resources.resourceCulture);
      }
    }

    internal static string ProxyException_FailedGetHostAddresses
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ProxyException_FailedGetHostAddresses), xNet.Resources.resourceCulture);
      }
    }

    internal static string ProxyException_NotSupportedAddressType
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ProxyException_NotSupportedAddressType), xNet.Resources.resourceCulture);
      }
    }

    internal static string ProxyException_ReceivedEmptyResponse
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ProxyException_ReceivedEmptyResponse), xNet.Resources.resourceCulture);
      }
    }

    internal static string ProxyException_ReceivedWrongResponse
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ProxyException_ReceivedWrongResponse), xNet.Resources.resourceCulture);
      }
    }

    internal static string ProxyException_ReceivedWrongStatusCode
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ProxyException_ReceivedWrongStatusCode), xNet.Resources.resourceCulture);
      }
    }

    internal static string ProxyException_Socks5_FailedAuthOn
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ProxyException_Socks5_FailedAuthOn), xNet.Resources.resourceCulture);
      }
    }

    internal static string ProxyException_WaitDataTimeout
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (ProxyException_WaitDataTimeout), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks_UnknownError
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks_UnknownError), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks4_CommandReplyRequestRejectedCannotConnectToIdentd
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks4_CommandReplyRequestRejectedCannotConnectToIdentd), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks4_CommandReplyRequestRejectedDifferentIdentd
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks4_CommandReplyRequestRejectedDifferentIdentd), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks4_CommandReplyRequestRejectedOrFailed
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks4_CommandReplyRequestRejectedOrFailed), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks5_AuthMethodReplyNoAcceptableMethods
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks5_AuthMethodReplyNoAcceptableMethods), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks5_CommandReplyAddressTypeNotSupported
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyAddressTypeNotSupported), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks5_CommandReplyCommandNotSupported
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyCommandNotSupported), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks5_CommandReplyConnectionNotAllowedByRuleset
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyConnectionNotAllowedByRuleset), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks5_CommandReplyConnectionRefused
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyConnectionRefused), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks5_CommandReplyGeneralSocksServerFailure
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyGeneralSocksServerFailure), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks5_CommandReplyHostUnreachable
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyHostUnreachable), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks5_CommandReplyNetworkUnreachable
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyNetworkUnreachable), xNet.Resources.resourceCulture);
      }
    }

    internal static string Socks5_CommandReplyTTLExpired
    {
      get
      {
        return xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyTTLExpired), xNet.Resources.resourceCulture);
      }
    }
  }
}
