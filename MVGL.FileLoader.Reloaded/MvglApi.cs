using MVGL.FileLoader.Interfaces;

namespace MVGL.FileLoader.Reloaded;

public class MvglApi(MvglModRegistry registry) : IMvglApi
{
    public int AddFolder(string folder) => registry.AddFolder(folder);

    public void BindFile(string bindPath, string file) => registry.BindFile(bindPath, file);
}