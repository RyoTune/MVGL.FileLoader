namespace MVGL.FileLoader.Interfaces.FileProcessors;

/// <summary>
/// Handled file entry.
/// </summary>
public class HandledFile
{
    /// <summary>
    /// File to be processed.
    /// </summary>
    public string FilePath { get; init; } = string.Empty;

    /// <summary>
    /// The initial bind path that was to be used for this file.
    /// </summary>
    public string InitialBindPath { get; init; } = string.Empty;
}