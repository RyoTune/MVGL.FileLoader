using System.Diagnostics.CodeAnalysis;
using MVGL.FileLoader.Interfaces;
using MVGL.FileLoader.Interfaces.FileProcessors;
using MVLibraryNET.MBE;

namespace MVGL.FileLoader.Reloaded.FileProcessors;

public class MbeProcessor : IFileProcessor
{
    private readonly IMvglApi _api;
    private readonly string _cacheDir;
    private readonly Dictionary<string, string> _mvglNameToPath;
    private readonly Dictionary<string, ActiveMbe> _activeMbes = new(StringComparer.OrdinalIgnoreCase);
    
    public MbeProcessor(IMvglApi api, string cacheDir)
    {
        _api = api;
        _cacheDir = cacheDir;
        _mvglNameToPath = api.GetMvglFilesInGameDir().ToDictionary(x =>
        {
            var mvglName = Path.GetFileName(x);
            mvglName = mvglName[..mvglName.IndexOf('.')];
            return mvglName;
        }, x => x, StringComparer.OrdinalIgnoreCase);
    }

    public Func<string, bool> CanHandle { get; } = file => file.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);
    
    public void Process(IReadOnlyList<HandledFile> files)
    {
        foreach (var file in files)
        {
            var mbePath = Path.GetDirectoryName(file.InitialBindPath)!;
            if (!TryGetActiveMbe(mbePath, out var activeMbe))
                continue;

            var sheetName = Path.GetFileNameWithoutExtension(file.FilePath);
            if (!activeMbe.BaseMbe.Sheets.TryGetValue(sheetName, out var ogSheet))
            {
                Log.Error($"{nameof(MbeProcessor)} || Sheet '{sheetName}' not found in MBE '{mbePath}'.\nFile: {file.FilePath}");
                continue;
            }

            // Generate diff against original MBE.
            var csvSheet = new Sheet(file.FilePath);
            var diff = ogSheet.GenerateDiff(csvSheet);
            
            // Apply diff to current MBE.
            activeMbe.CurrentMbe.Sheets[sheetName].MergeDiff(diff);
        }

        // Write and bind MBEs.
        foreach (var (mbePath, value) in _activeMbes)
        {
            var currentMbe = value.CurrentMbe;
            
            var outputFile = Path.Join(_cacheDir, mbePath);
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
            using var outStream = File.Create(outputFile);
            
            currentMbe.Write(outStream);
            _api.BindFile(mbePath, outputFile);
        }
    }

    private bool TryGetActiveMbe(string mbePath, [NotNullWhen(true)] out ActiveMbe? activeMbe)
    {
        // mbePath = app0_text/data/example.mbe
        if (_activeMbes.TryGetValue(mbePath, out activeMbe)) return true;
        
        var mvglName = mbePath[..mbePath.IndexOf('\\')];
        if (!_mvglNameToPath.TryGetValue(mvglName, out var mvglFilePath))
        {
            Log.Error($"{nameof(MbeProcessor)} || MVGL '{mvglName}' not found.\nFile: {mbePath}");
            return false;
        }
            
        var mvglFiles = _api.GetMvglFilesCached(mvglFilePath);
        var mbeFileName = mbePath[(mbePath.IndexOf('\\') + 1)..];
        if (!mvglFiles.FilesByFileName.TryGetValue(mbeFileName, out var fileIdx))
        {
            Log.Error($"{nameof(MbeProcessor)} || MBE '{mbeFileName}' not found in MVGL '{mvglName}'.\nFile: {mbePath}");
            return false;
        }
            
        using var reader = _api.GetMvLib().CreateMvglReader(File.OpenRead(mvglFilePath), true);
        using var ms = new MemoryStream(reader.ExtractFile(mvglFiles.Files[fileIdx].File).Span.ToArray());
        var baseMbe = new Mbe(ms, false);
        ms.Position = 0;
        var currMbe = new Mbe(ms, false);

        activeMbe = new(baseMbe, currMbe);
        _activeMbes[mbePath] = activeMbe;

        return true;
    }

    private record ActiveMbe(Mbe BaseMbe, Mbe CurrentMbe);
}
