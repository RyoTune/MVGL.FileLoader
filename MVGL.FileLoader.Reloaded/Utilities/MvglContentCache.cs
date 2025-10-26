using MVGL.FileLoader.Interfaces.Structs;
using MVLibraryNET;

namespace MVGL.FileLoader.Reloaded.Utilities;

public class MvglContentCache
{
    private readonly Dictionary<string, MvglCacheEntry> _pathToEntry = new();

    /// <summary>
    /// Gets contents of MVGL file from the cache.
    /// </summary>
    /// <param name="filePath">Path to the CPK file.</param>
    public MvglCacheEntry Get(string filePath)
    {
        var normalizedPath = Path.GetFullPath(filePath);
        if (_pathToEntry.TryGetValue(normalizedPath, out var result))
            return result;

        return CacheMvgl(normalizedPath);
    }

    private MvglCacheEntry CacheMvgl(string normalizedPath)
    {
        using var stream = new FileStream(normalizedPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        var reader  = MVLibrary.Instance.CreateMvglReader(stream, false);
        var files   = reader.GetFiles();
        
        var array = GC.AllocateUninitializedArray<CachedMvglFile>(files.Length);
        var relativePathDictionary = new Dictionary<string, int>(files.Length, StringComparer.OrdinalIgnoreCase);
        var fileNameDictionary = new Dictionary<string, int>(files.Length, StringComparer.OrdinalIgnoreCase);
        
        for (int x = 0; x < files.Length; x++)
        {
            ref var file = ref files[x];
            var cachedFile = new CachedMvglFile
            {
                File = file,
                FileName = file.FileName,
            };

            array[x] = cachedFile;
            relativePathDictionary[cachedFile.FileName] = x;
            fileNameDictionary[file.FileName] = x;
        }

        var result = new MvglCacheEntry
        {
            Files = array,
            FilesByPath = relativePathDictionary,
            FilesByFileName = fileNameDictionary,
            LastModified = File.GetLastWriteTime(stream.SafeFileHandle)
        };
        
        _pathToEntry[normalizedPath] = result;
        return result;
    }

    /// <summary>
    /// Clears the cache.
    /// </summary>
    public void Clear() => _pathToEntry.Clear();
}