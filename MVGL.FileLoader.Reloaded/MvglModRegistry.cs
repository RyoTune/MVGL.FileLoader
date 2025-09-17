namespace MVGL.FileLoader.Reloaded;

public class MvglModRegistry
{
    private readonly Dictionary<string, DstsFile> _dstsFiles = new(StringComparer.OrdinalIgnoreCase);

    public int AddFolder(string folder)
    {
        var numFiles = 0;
        foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
        {
            var relPath = Path.GetRelativePath(folder, file).Replace('\\', '/');
            BindFile(relPath, file);
            numFiles++;
        }

        return numFiles;
    }

    public void BindFile(string bindPath, string file)
    {
        _dstsFiles[bindPath] = new(file, new FileInfo(file).Length);
        Log.Debug($"Registered File: {file}\nPath: {bindPath}");
    }

    public bool TryGetFile(string gameFile, out DstsFile dstsFile)
    {
        if (_dstsFiles.TryGetValue(gameFile, out dstsFile)) return true;
        return false;
    }

    public readonly record struct DstsFile(string Path, long Size);
}