using Shared.Models;
namespace SharedBinance.Models
{
    public struct MarketUpdate
    {
        public UpdateAction Action;
        public EntryType Type;
        public double Price;
        public double Volume;
        public string EntryID;
    }
}
