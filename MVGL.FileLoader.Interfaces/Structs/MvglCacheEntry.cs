namespace MVGL.FileLoader.Interfaces.Structs;

/// <summary>
/// Represents an individual entry storing cached data for each MVGL.
/// </summary>
public class MvglCacheEntry
{
    /// <summary>
    /// The files stores for this MVGL.
    /// </summary>
    public CachedMvglFile[] Files { get; init; } = [];
    
    /// <summary>
    /// Last time MVGL was modified, can be used to invalidate custom user cache.
    /// </summary>
    public DateTime LastModified { get; init; }

    /// <summary>
    /// Contains a map of all relative paths in MVGL to corresponding index in <see cref="Files"/> array.
    /// Directory separators normalized to <see cref="Path.DirectorySeparatorChar"/>.
    /// </summary>
    public Dictionary<string, int> FilesByPath { get; init; } = [];

    /// <summary>
    /// Contains a map of all file names corresponding to index in <see cref="Files"/> array.
    /// For fast lookup.
    /// </summary>
    public Dictionary<string, int> FilesByFileName { get; init; } = [];
}