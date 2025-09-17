namespace MVGL.FileLoader.Interfaces;

public interface IMvglApi
{
    int AddFolder(string folder);

    void BindFile(string bindPath, string file);
}