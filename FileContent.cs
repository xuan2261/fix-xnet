// Decompiled with JetBrains decompiler
// Type: xNet.FileContent
// Assembly: xNet, Version=3.3.3.0, Culture=neutral, PublicKeyToken=null
// MVID: BCFC550F-93AE-4DF9-8F50-A984FB298337
// Assembly location: C:\Users\Henris\Desktop\Smart Pastebin\xNet-0bfa2388b222842ad29fcffb3677177a38854ebd\bin\Release\fsdfsd.dll

using System;
using System.IO;

namespace xNet
{
  public class FileContent : StreamContent
  {
    public FileContent(string pathToContent, int bufferSize = 32768)
    {
      if (pathToContent == null)
        throw new ArgumentNullException(nameof (pathToContent));
      if (pathToContent.Length == 0)
        throw ExceptionHelper.EmptyString(nameof (pathToContent));
      if (bufferSize < 1)
        throw ExceptionHelper.CanNotBeLess<int>(nameof (bufferSize), 1);
      this._content = (Stream) new FileStream(pathToContent, FileMode.Open, FileAccess.Read);
      this._bufferSize = bufferSize;
      this._initialStreamPosition = 0L;
      this._contentType = Http.DetermineMediaType(Path.GetExtension(pathToContent));
    }
  }
}
