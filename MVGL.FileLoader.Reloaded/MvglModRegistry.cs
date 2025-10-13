using System.Diagnostics.CodeAnalysis;

namespace MVGL.FileLoader.Reloaded;

public class MvglModRegistry
{
    private readonly Dictionary<string, string> _mvglFiles = new(StringComparer.OrdinalIgnoreCase);

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
        _mvglFiles[bindPath] = file;
        Log.Debug($"Registered File: {file}\nPath: {bindPath}");
    }

    public bool TryGetFile(string gameFile, [NotNullWhen(true)]out string? modFile)
    {
        if (_mvglFiles.TryGetValue(gameFile, out modFile)) return true;
        
        // For .img files, also allow DDS files.
        if (gameFile.EndsWith(".img", StringComparison.OrdinalIgnoreCase) &&
            _mvglFiles.TryGetValue(Path.ChangeExtension(gameFile, ".dds"), out modFile)) return true;
        
        return false;
    }
}