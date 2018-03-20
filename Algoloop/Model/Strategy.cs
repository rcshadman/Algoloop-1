using QuantConnect.Securities;

namespace Algoloop.Model
{
    public class Strategy
    {
        public SyncObservableCollection<Security> Securities { get; set; }

        public Strategy()
        {
            Securities = new SyncObservableCollection<Security>();
        }
    }
}
