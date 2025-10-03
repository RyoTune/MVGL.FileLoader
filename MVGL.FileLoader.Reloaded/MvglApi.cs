using MVGL.FileLoader.Interfaces;
using Reloaded.Mod.Interfaces;

namespace MVGL.FileLoader.Reloaded;

public class MvglApi : IMvglApi
{
    private readonly MvglModRegistry _registry;
    private readonly HashSet<string> _probePaths = new(StringComparer.OrdinalIgnoreCase);

    public MvglApi(IModLoader modLoader, MvglModRegistry registry)
    {
        _registry = registry;
        modLoader.ModLoaded += (_, mod) =>
        {
            if (!Project.IsModDependent(mod)) return;
            
            var modDir = modLoader.GetDirectoryForModId(mod.ModId);
            var numModFiles = _probePaths.Select(path => Path.Join(modDir, path)).Where(Directory.Exists).Sum(probeDir => _registry.AddFolder(probeDir));
            
            if (numModFiles > 0)
                Log.Information($"Registered Mod: {mod.ModName} || Total Files: {numModFiles}");
        };
    }

    public void AddProbingPath(string path) => _probePaths.Add(path);

    public int AddFolder(string folder) => _registry.AddFolder(folder);

    public void BindFile(string bindPath, string file) => _registry.BindFile(bindPath, file);
}