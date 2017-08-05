using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HitBtcOrchestrator
{
    public class MarketDataSnapshotFullRefresh : INotifyPropertyChanged
    {
        private double _maxAskCoinPrice;
        private double _minAskCoinPrice;
        private double _maxBidCoinPrice;
        private double _minBidCoinPrice;
        public float TotalAskCoin { get; set; }
        public float TotalBidCoin { get; set; }
        public double AvgAskCoinPrice { get; set; }
        public double AvgBidCoinPrice { get; set; }

        public double TotalAskCoinValue { get; set; }
        public double TotalBidCoinValue { get; set; }

        public int snapshotSeqNo { get; set; }
        public string symbol { get; set; }
        public string exchangeStatus { get; set; }
        public Ask[] ask { get; set; }
        public Bid[] bid { get; set; }
        public string FirstCurrency { get; set; }
        public string LastCurrency { get; set; }


        public void SetTotalCoins(float minAskCoinPrice, float maxAskCoinPrice, float minBidCoinPrice, float maxBidCoinPrice)
        {
            AskCalculation(minAskCoinPrice, maxAskCoinPrice);
            BidCalculation(minBidCoinPrice, maxBidCoinPrice);
        }

        private void AskCalculation(float minAskCoinPrice, float maxAskCoinPrice)
        {
            var askNotEmpty = ask.Any();
            MinAskCoinPrice = askNotEmpty ? ask.Min(m => m.price) : 0f;
            MaxAskCoinPrice = askNotEmpty ? ask.Max(m => m.price) : 0f;

            var askItems = ask.Where(w => w.price >= minAskCoinPrice && w.price <= maxAskCoinPrice).ToArray();
            TotalAskCoin = askItems.Sum(s => s.size);
            foreach (var askItem in askItems)
            {
                if (TotalAskCoin > 0)
                {
                    askItem.PercentImpact = 100 * askItem.size / TotalAskCoin;
                    askItem.Ratio = askItem.PercentImpact * askItem.price;
                }
                askItem.TotalValue = askItem.size * askItem.price;
            }
            ask = askItems.OrderByDescending(o => o.Ratio).ToArray();
            askNotEmpty = ask.Any();
            AvgAskCoinPrice = askNotEmpty && TotalAskCoin > 0 ? ask.Sum(a => a.TotalValue) / TotalAskCoin : 0f;
            TotalAskCoinValue = ask.Sum(s => s.TotalValue);
        }

        public double MaxAskCoinPrice
        {
            get { return _maxAskCoinPrice; }
            set { _maxAskCoinPrice = value; OnPropertyChanged(); }
        }

        public double MinAskCoinPrice
        {
            get { return _minAskCoinPrice; }
            set { _minAskCoinPrice = value; OnPropertyChanged();}
        }

        private void BidCalculation(float minBidCoinPrice, float maxBidCoinPrice)
        {
            var bidNotEmpty = bid.Any();
            MinBidCoinPrice = bidNotEmpty ? bid.Min(m => m.price) : 0f;
            MaxBidCoinPrice = bidNotEmpty ? bid.Max(m => m.price) : 0f;
            var bids = bid.Where(w => w.price >= minBidCoinPrice && w.price <= maxBidCoinPrice).ToArray();
            TotalBidCoin = bids.Sum(s => s.size);
            foreach (var bidItem in bids)
            {
                if (TotalBidCoin > 0)
                {
                    bidItem.PercentImpact = 100 * bidItem.size / TotalBidCoin;
                    bidItem.Ratio = bidItem.PercentImpact * bidItem.price;
                }
                bidItem.TotalValue = bidItem.size * bidItem.price;

            }
            bid = bids.OrderByDescending(o => o.Ratio).ToArray();

            AvgBidCoinPrice = bid.Any() && TotalBidCoin > 0 ? bid.Sum(a => a.TotalValue) / TotalBidCoin : 0f;
            TotalBidCoinValue = bid.Sum(s => s.TotalValue);
        }

        public double MaxBidCoinPrice
        {
            get { return _maxBidCoinPrice; }
            set { _maxBidCoinPrice = value; OnPropertyChanged();}
        }

        public double MinBidCoinPrice
        {
            get { return _minBidCoinPrice; }
            set { _minBidCoinPrice = value; OnPropertyChanged(); }
        }

        public DateTime LastUpdate { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}