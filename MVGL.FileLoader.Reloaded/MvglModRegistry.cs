using System.Diagnostics.CodeAnalysis;
using MVGL.FileLoader.Interfaces.FileProcessors;

namespace MVGL.FileLoader.Reloaded;

public class MvglModRegistry
{
    private readonly Dictionary<string, string> _mvglFiles = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<IFileProcessor, List<HandledFile>> _processors = [];

    public int AddFolder(string folder)
    {
        var numFiles = 0;
        foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
        {
            numFiles++;

            var relPath = Path.GetRelativePath(folder, file);

            var isHandled = false;
            foreach (var proc in _processors)
            {
                if (!proc.Key.CanHandle(file)) continue;
                proc.Value.Add(new() { InitialBindPath = relPath, FilePath = file });
                isHandled = true;
                Log.Debug($"File handled by processor '{proc.Key.GetType().Name}'.\nFile: {file}");
                break;
            }

            if (isHandled) continue;
            BindFile(relPath, file);
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

    public void RegisterProcessor(IFileProcessor processor) => _processors[processor] = [];

    public void ProcessFiles()
    {
        foreach (var kvp in _processors)
            kvp.Key.Process(kvp.Value);
    }
}