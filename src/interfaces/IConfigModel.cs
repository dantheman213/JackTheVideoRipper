namespace JackTheVideoRipper;

public interface IConfigModel
{
    string Filepath { get; }

    bool ExistsOnDisk { get; }
    
    void WriteToDisk();

    T? GetFromDisk<T>() where T : ConfigModel;

    void Validate();
}