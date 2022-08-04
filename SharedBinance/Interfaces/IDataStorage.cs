using SharedBinance.interfaces;

namespace Shared.Interfaces
{
    public interface IDataStorage : ISettingStorage
    {
        bool Set(string key, string value);
        string[] AvailableKeys { get; }
    }
}
