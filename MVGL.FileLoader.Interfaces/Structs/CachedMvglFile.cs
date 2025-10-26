using MVLibraryNET.Definitions.MVGL.Structs;

namespace MVGL.FileLoader.Interfaces.Structs;

/// <summary>
/// A <see cref="MvglFile"/> obtained from the cache.
/// </summary>
public class CachedMvglFile
{
    /// <summary>
    /// File name inside the MVGL, normalized to use system path separators.
    /// </summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>
    /// The MVGL file entry.
    /// </summary>
    public MvglFile File { get; init; }
}