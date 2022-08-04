using SharedBinance.Models;
namespace SharedBinance.interfaces
{
    public interface ISynchroMarketBook
    {
        void GetInitialSnapshot(DepthSnapshot snapshot);
        void GetUpdate(DepthSnapshot snapshot);
        void Clear();

        bool GetInitialSnapshotFlag();
    }
}
