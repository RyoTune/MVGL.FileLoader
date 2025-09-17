using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Reloaded.Hooks.Definitions;
// ReSharper disable InconsistentNaming

namespace MVGL.FileLoader.Reloaded;

public unsafe class MvglModLoader
{
    
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool ReadFile(
        SafeFileHandle hFile,
        IntPtr lpBuffer,
        int nNumberOfBytesToRead,
        out int lpNumberOfBytesRead,
        IntPtr lpOverlapped);

    private delegate byte PackFileResource_ReadFile(nint filePath, nint buffer, int size);
    private readonly SHFunction<PackFileResource_ReadFile> _ReadFile;

    private delegate nint PackFileResource_GetFileSizeV1(nint param_1, nint filePath, long* size, nint param_4, int param_5);
    private delegate nint PackFileResource_GetFileSizeV2(nint filePath, long* size);

    private IHook<PackFileResource_GetFileSizeV1>? _getFileSizeV1;
    private IHook<PackFileResource_GetFileSizeV2>? _getFileSizeV2;
    
    private readonly MvglModRegistry _registry;
    
    public MvglModLoader(MvglModRegistry registry)
    {
        _registry = registry;
        
        _ReadFile = new(ReadFileImpl);
        Project.Scans.AddScanHook("PackFileResource_GetFileSize", (result, hooks) =>
        {
            if (Project.Inis.TryGetSetting<int>("settings", "PackFileResource_GetFileSize_Version", "hooks", out var version))
            {
                switch (version)
                {
                    case 1:
                        _getFileSizeV1 = hooks.CreateHook<PackFileResource_GetFileSizeV1>(GetFileSizeV1Impl, result).Activate();
                        break;
                    case 2:
                        _getFileSizeV2 = hooks.CreateHook<PackFileResource_GetFileSizeV2>(GetFileSizeV2Impl, result).Activate();
                        break;
                }
            }
        });
    }

    private byte ReadFileImpl(nint filePath, nint buffer, int size)
    {
        var filePathStr = Marshal.PtrToStringAnsi(filePath)!;
        if (Mod.Config.DevMode)
        {
            Log.Information($"{nameof(PackFileResource_ReadFile)} || File: {filePathStr}");
        }
        else
        {
            Log.Debug($"{nameof(PackFileResource_ReadFile)} || File: {filePathStr}");
        }
        
        if (_registry.TryGetFile(filePathStr, out var modFile))
        {
            Log.Debug($"{nameof(PackFileResource_ReadFile)} || Replacing: {filePath}\nFile: {modFile}");
            
            using var fs = new FileStream(modFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (ReadFile(fs.SafeFileHandle, buffer, (int)GetFileSize(modFile), out _, nint.Zero))
            {
                return 1;
            }
            
            Log.Error($"ReadFile failed.\nFile: {modFile}");
        }

        return _ReadFile.Hook!.OriginalFunction(filePath, buffer, size);
    }

    private readonly Dictionary<string, long> _fileSizes = [];

    private nint GetFileSizeV1Impl(nint param_1, nint filePath, long* size, nint param_4, int param_5) => TrySetFileSize(filePath, size) ? 1 : _getFileSizeV1!.OriginalFunction(param_1, filePath, size, param_4, param_5);

    private nint GetFileSizeV2Impl(nint filePath, long* size) => TrySetFileSize(filePath, size) ? 1 : _getFileSizeV2!.OriginalFunction(filePath, size);

    private bool TrySetFileSize(nint filePath, long* size)
    {
        var filePathStr = Marshal.PtrToStringAnsi(filePath)!;
        if (_registry.TryGetFile(filePathStr, out var modFile))
        {
            *size = GetFileSize(modFile);
            return true;
        }

        return false;
    }
    
    private long GetFileSize(string file)
    {
        if (_fileSizes.TryGetValue(file, out var prev)) return prev;
        _fileSizes[file] = new FileInfo(file).Length;
        return _fileSizes[file];
    }
}