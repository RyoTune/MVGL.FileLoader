using System.Runtime.InteropServices;
using FileEmulationFramework.Lib.IO;
using MVGL.FileLoader.Interfaces;
using MVGL.FileLoader.Interfaces.FileProcessors;
using MVGL.FileLoader.Interfaces.Structs;
using MVGL.FileLoader.Reloaded.Utilities;
using MVLibraryNET;
using MVLibraryNET.Definitions;
using Reloaded.Mod.Interfaces;

namespace MVGL.FileLoader.Reloaded;

public class MvglApi : IMvglApi
{
    private readonly MvglModRegistry _registry;
    private readonly string _gamePath;
    private readonly MvglContentCache _mvglContentCache;
    private readonly HashSet<string> _probePaths = new(StringComparer.OrdinalIgnoreCase);
    private string[]? _mvglFiles;

    public MvglApi(IModLoader modLoader, MvglModRegistry registry)
    {
        _registry = registry;
        _gamePath = modLoader.GetAppConfig().AppLocation;
        _mvglContentCache = new();
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
    
    public void RegisterProcessor(IFileProcessor processor) => _registry.RegisterProcessor(processor);

    public MvglCacheEntry GetMvglFilesCached(string filePath) => _mvglContentCache.Get(filePath);

    public string[] GetMvglFilesInGameDir()
    {
        if (_mvglFiles != null)
            return _mvglFiles;
        
        // Note: In some cases, applications might store binaries in subfolders.
        // We will go down folders until we find a MVGL file.
        var currentFolder = _gamePath;
        var results = new List<string>();
        var fileInfo = new List<FileInformation>();
        var directoryInfo = new List<DirectoryInformation>();
        
        do
        {
            currentFolder = Path.GetDirectoryName(currentFolder);
            if (currentFolder == null)
                return [];
            
            fileInfo.Clear();
            directoryInfo.Clear();
#pragma warning disable CA1416
            WindowsDirectorySearcher.GetDirectoryContentsRecursive(currentFolder, fileInfo, directoryInfo);
#pragma warning restore CA1416

            foreach (var file in CollectionsMarshal.AsSpan(fileInfo))
            {
                if (file.FileName.EndsWith(".mvgl", StringComparison.OrdinalIgnoreCase))
                    results.Add(Path.GetFullPath(Path.Combine(file.DirectoryPath, file.FileName)));
            }
        } 
        while (results.Count <= 0);

        _mvglFiles = results.ToArray();
        return _mvglFiles;
    }

    public IMVLibrary GetMvLib() => MVLibrary.Instance;
}