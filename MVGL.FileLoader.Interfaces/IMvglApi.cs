using MVGL.FileLoader.Interfaces.FileProcessors;
using MVGL.FileLoader.Interfaces.Structs;
using MVLibraryNET.Definitions;

namespace MVGL.FileLoader.Interfaces;

/// <summary>
/// MVGL File Loader API.
/// </summary>
public interface IMvglApi
{
    /// <summary>
    /// Add a new folder to load files from in every mod, like the default <c>mvgl-loader</c>.
    /// </summary>
    /// <param name="path">Probe path.</param>
    /// <returns>Amount of files loaded.</returns>
    void AddProbingPath(string path);
    
    /// <summary>
    /// Add a folder to load files from.
    /// </summary>
    /// <param name="folder">Folder path.</param>
    /// <returns>Amount of files loaded.</returns>
    int AddFolder(string folder);

    /// <summary>
    /// Bind a file to a path.
    /// </summary>
    /// <param name="bindPath">Path to bind.</param>
    /// <param name="file">File being bound.</param>
    void BindFile(string bindPath, string file);

    /// <summary>
    /// Register a file processor.
    /// </summary>
    /// <param name="processor">File processor.</param>
    public void RegisterProcessor(IFileProcessor processor);
    
    /// <summary>
    /// Gets the file contents of a given MVGL file.
    /// Data can be read from this MVGL file by using <see cref="IMVLibrary.CreateMvglReader"/> with FileStream.
    /// 
    /// This is an optimisation/speedup to prevent repeat parsing when multiple mods need to extract files.
    /// This cache is cleared after each rebuild.
    /// </summary>
    MvglCacheEntry GetMvglFilesCached(string filePath);
    
    /// <summary>
    /// Returns list of all MVGL files in game directory.
    /// This list is cached, used to speed up rebuilds.
    /// </summary>
    string[] GetMvglFilesInGameDir();
    
    /// <summary>
    /// Gets an instance of the MVLibrary.
    /// </summary>
    IMVLibrary GetMvLib();
}