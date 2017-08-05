using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HitBtcOrchestrator
{
    public class MyOwnCoin :INotifyPropertyChanged
    {
        private MarketDataSnapshotFullRefresh _currentUsdMarketData;
        private MarketDataSnapshotFullRefresh _currentBtcMarketData;
        private float _btcMinAskValue;
        private float _btcMaxAskValue;
        private float _btcMinBidValue;
        private float _btcMaxBidValue;
        public string CoinSymbol { get; set; }

        public float OwnedCoinQty { get; set; }
        public float OwnedCoinUsdValue { get; set; }
        public float OwnedCoinEurValue { get; set; }
        public float OwnedCoinBtcValue { get; set; }
        public float OwnedCoinEthValue { get; set; }

        public float UsdMinAskValue { get; set; }
        public float UsdMaxAskValue { get; set; }
        public float UsdMinBidValue { get; set; }
        public float UsdMaxBidValue { get; set; }

        public float EurMinAskValue { get; set; }
        public float EurMaxAskValue { get; set; }
        public float EurMinBidValue { get; set; }
        public float EurMaxBidValue { get; set; }

        public float BtcMinAskValue
        {
            get { return _btcMinAskValue; }
            set { _btcMinAskValue = value; OnPropertyChanged(); }
        }

        public float BtcMaxAskValue
        {
            get { return _btcMaxAskValue; }
            set { _btcMaxAskValue = value; OnPropertyChanged(); }
        }

        public float BtcMinBidValue
        {
            get { return _btcMinBidValue; }
            set { _btcMinBidValue = value; OnPropertyChanged(); }
        }

        public float BtcMaxBidValue
        {
            get { return _btcMaxBidValue; }
            set { _btcMaxBidValue = value; OnPropertyChanged(); }
        }

        public float EthMinAskValue { get; set; }
        public float EthMaxAskValue { get; set; }
        public float EthMinBidValue { get; set; }
        public float EthMaxBidValue { get; set; }

        public MarketDataSnapshotFullRefresh CurrentEthMarketData { get; set; }

        public MarketDataSnapshotFullRefresh CurrentBtcMarketData
        {
            get { return _currentBtcMarketData; }
            set { _currentBtcMarketData = value; OnPropertyChanged();}
        }

        public MarketDataSnapshotFullRefresh CurrentEurMarketData { get; set; }

        public MarketDataSnapshotFullRefresh CurrentUsdMarketData
        {
            get { return _currentUsdMarketData; }
            set { _currentUsdMarketData = value; OnPropertyChanged();}
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}