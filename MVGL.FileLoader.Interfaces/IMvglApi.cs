namespace MVGL.FileLoader.Interfaces;

/// <summary>
/// MVGL File Loader API.
/// </summary>
public interface IMvglApi
{
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
}