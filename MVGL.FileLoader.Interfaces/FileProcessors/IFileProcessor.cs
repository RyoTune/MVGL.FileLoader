namespace MVGL.FileLoader.Interfaces.FileProcessors;

/// <summary>
/// File processor interface.
/// </summary>
public interface IFileProcessor
{
    /// <summary>
    /// Func to determine whether a given file can be handled by this processor.
    /// </summary>
    Func<string, bool> CanHandle { get; }

    /// <summary>
    /// Executes the processor with the list of all handled files.
    /// </summary>
    /// <param name="files">Files that were handled.</param>
    void Process(IReadOnlyList<HandledFile> files);
}