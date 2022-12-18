using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.SharedModels;

internal sealed class ValidFileInfo
{
    /// <summary>
    /// file info
    /// </summary>
    public FileInfo FileInfo { get; private set; }

    public ValidFileInfo(string path, Texts texts)
    {
        path = Path.GetFullPath(path);
        FileInfo = new(path);
        if (!FileInfo.Exists)
            throw new ArgumentException(texts._("FileNotFound", path));
    }
}
