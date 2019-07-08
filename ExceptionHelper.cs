// Decompiled with JetBrains decompiler
// Type: xNet.ExceptionHelper
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: 8FAB7F03-1085-4650-8C57-7A04F40293E8
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet.dll

using System;

namespace xNet
{
  internal static class ExceptionHelper
  {
    internal static ArgumentException EmptyString(string paramName)
    {
      return new ArgumentException(Resources.ArgumentException_EmptyString, paramName);
    }

    internal static ArgumentOutOfRangeException CanNotBeLess<T>(
      string paramName,
      T value)
      where T : struct
    {
      return new ArgumentOutOfRangeException(paramName, string.Format(Resources.ArgumentOutOfRangeException_CanNotBeLess, (object) value));
    }

    internal static ArgumentOutOfRangeException CanNotBeGreater<T>(
      string paramName,
      T value)
      where T : struct
    {
      return new ArgumentOutOfRangeException(paramName, string.Format(Resources.ArgumentOutOfRangeException_CanNotBeGreater, (object) value));
    }

    internal static ArgumentException WrongPath(
      string paramName,
      Exception innerException = null)
    {
      return new ArgumentException(Resources.ArgumentException_WrongPath, paramName, innerException);
    }

    internal static ArgumentOutOfRangeException WrongTcpPort(
      string paramName)
    {
      if (paramName == null)
        throw new ArgumentNullException(nameof (paramName));
      return new ArgumentOutOfRangeException("port", string.Format(Resources.ArgumentOutOfRangeException_CanNotBeLessOrGreater, (object) 1, (object) (int) ushort.MaxValue));
    }

    internal static bool ValidateTcpPort(int port)
    {
      return port >= 1 && port <= (int) ushort.MaxValue;
    }
  }
}
