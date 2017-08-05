using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebSocketSharp;

namespace HitBtcOrchestrator
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private MarketDataSnapshotFullRefresh _btcUsdCurrentAvgPrice;
        private MarketDataSnapshotFullRefresh _ethUsdCurrentAvgPrice;

        private float _ethEurMaxBidValue;
        private float _btcUsdMinAskValue;
        private float _btcUsdMaxAskValue;
        private float _btcUsdMinBidValue;
        private float _btcUsdMaxBidValue;
        private bool _isConnected;
        private bool _isSocketReady;
        private bool _connectionInitialized;
        private DateTime _lastMessage;
        private ObservableCollection<MyOwnCoin> _myOwnCoins;
        private MarketDataSnapshotFullRefresh _btcEurCurrentAvgPrice;
        private MarketDataSnapshotFullRefresh _ethEurCurrentAvgPrice;
        private float _ethEurMinBidValue;
        private float _ethEurMaxAskValue;
        private float _ethEurMinAskValue;
        private float _btcEurMinAskValue;
        private float _btcEurMinBidValue;
        private float _btcEurMaxAskValue;
        private float _btcEurMaxBidValue;
        private float _ethUsdMinAskValue;
        private float _ethUsdMaxAskValue;
        private float _ethUsdMinBidValue;
        private float _ethUsdMaxBidValue;
        public bool AutoReconnectOnCloseAndError { get; set; } = true;

        public MainWindowViewModel()
        {
            MyOwnCoins = new ObservableCollection<MyOwnCoin>() { new MyOwnCoin() { CoinSymbol = "BCC" }, new MyOwnCoin() { CoinSymbol = "ETH" }, new MyOwnCoin() { CoinSymbol = "BCN" } };
            InitWsConnection();
        }

        private void InitWsConnection()
        {
            WebSocket?.Dispose();

            WebSocket = new WebSocket("ws://api.hitbtc.com:80", onMessage: OnMessage, onOpen: OnOpen, onError: OnError, onClose: OnClose);

            WebSocket.Connect().ContinueWith(task =>
            {
                ConnectionInitialized = true;
                IsReconnecting = false;
            });
        }

        private Task OnClose(CloseEventArgs closeEventArgs)
        {
            ConnectionInitialized = false;
            IsSocketReady = false;

            return Task.FromResult(0);
        }

        public DateTime LastMessage
        {
            get => _lastMessage;
            set { _lastMessage = value; OnPropertyChanged(); }
        }

        public bool ConnectionInitialized
        {
            get => _connectionInitialized;
            set { _connectionInitialized = value; OnPropertyChanged(); }
        }

        public bool IsSocketReady
        {
            get => _isSocketReady;
            set { _isSocketReady = value; OnPropertyChanged(); }
        }

        public bool IsConnected
        {
            get => _isConnected;
            set { _isConnected = value; OnPropertyChanged(); }
        }

        public float BtcUsdMinAskValue
        {
            get => _btcUsdMinAskValue;
            set
            {
                _btcUsdMinAskValue = value;
                OnPropertyChanged();
            }
        }

        public float BtcUsdMaxAskValue
        {
            get => _btcUsdMaxAskValue;
            set
            {
                _btcUsdMaxAskValue = value;
                OnPropertyChanged();
            }
        }

        public float BtcUsdMinBidValue
        {
            get => _btcUsdMinBidValue;
            set { _btcUsdMinBidValue = value; OnPropertyChanged(); }
        }

        public float BtcUsdMaxBidValue
        {
            get => _btcUsdMaxBidValue;
            set { _btcUsdMaxBidValue = value; OnPropertyChanged(); }
        }

        public MarketDataSnapshotFullRefresh BtcUsdCurrentAvgPrice
        {
            get => _btcUsdCurrentAvgPrice;
            set { _btcUsdCurrentAvgPrice = value; OnPropertyChanged(); }
        }


        public float EthUsdMinAskValue
        {
            get { return _ethUsdMinAskValue; }
            set { _ethUsdMinAskValue = value; OnPropertyChanged(); }
        }

        public float EthUsdMaxAskValue
        {
            get { return _ethUsdMaxAskValue; }
            set { _ethUsdMaxAskValue = value; OnPropertyChanged(); }
        }

        public float EthUsdMinBidValue
        {
            get { return _ethUsdMinBidValue; }
            set { _ethUsdMinBidValue = value; OnPropertyChanged(); }
        }

        public float EthUsdMaxBidValue
        {
            get { return _ethUsdMaxBidValue; }
            set { _ethUsdMaxBidValue = value;  OnPropertyChanged();}
        }

        public MarketDataSnapshotFullRefresh EthUsdCurrentAvgPrice
        {
            get => _ethUsdCurrentAvgPrice;
            set { _ethUsdCurrentAvgPrice = value; OnPropertyChanged(); }
        }



        private Task OnError(ErrorEventArgs errorEventArgs)
        {
            Debug.WriteLine(errorEventArgs.Message);
            ConnectionInitialized = false;
            IsSocketReady = false;
            //if (AutoReconnectOnCloseAndError)
            //{
            //    InitWsConnection();
            //}
            return Task.FromResult(0);
        }

        private Task OnOpen()
        {
            IsSocketReady = true;
            if (!IsPingPollStarted)
            {
                IsPingPollStarted = true;
                PollingTimer = new Timer(PollPingCallback, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));

            }
            return Task.FromResult(0);
        }

        public Timer PollingTimer { get; set; }

        private void PollPingCallback(object state)
        {
            WebSocket?.Ping().ContinueWith(task =>
            {
                if (!task.Result)
                {
                    if (!IsReconnecting)
                    {

                        IsReconnecting = true;

                        ConnectionInitialized = false;
                        IsSocketReady = false;
                        InitWsConnection();
                    }
                }
            });

        }

        public bool IsReconnecting { get; set; }

        public bool IsPingPollStarted { get; set; }

        private Task OnMessage(MessageEventArgs messageEventArgs)
        {
            return Task.Run(() =>
            {
                if (messageEventArgs.Opcode != Opcode.Text)
                {
                    return Task.FromResult(0);
                }

                var readToEnd = messageEventArgs.Text.ReadToEnd();
                if (readToEnd.StartsWith($"{{\"MarketDataSnapshotFullRefresh\":"))
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        Formatting = Formatting.None,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        Converters = new List<JsonConverter> { new FloatJsConverter() }
                    };
                    var tobj = JsonConvert.DeserializeObject<MarketDataSnapshotFullResponse>(readToEnd, settings);
                    var marketData = tobj.MarketDataSnapshotFullRefresh;
                    var hitBtcPair = marketData.symbol;
                    var hitBtcExhangeStatus = marketData.exchangeStatus;
                    var hitBtcExchangeWorks = hitBtcExhangeStatus == "working";
                    if (hitBtcPair == "BTCUSD" && hitBtcExchangeWorks)
                    {
                        marketData.LastUpdate = DateTime.Now;
                        marketData.FirstCurrency = "BTC";
                        marketData.LastCurrency = "USD";
                        marketData.SetTotalCoins(BtcUsdMinAskValue, BtcUsdMaxAskValue, BtcUsdMinBidValue, BtcUsdMaxBidValue);

                        BtcUsdCurrentAvgPrice = marketData;

                        LastMessage = DateTime.Now;
                    }
                    if (hitBtcPair == "ETHUSD" && hitBtcExchangeWorks)
                    {
                        marketData.FirstCurrency = "ETH";
                        marketData.LastCurrency = "USD";
                        marketData.SetTotalCoins(EthUsdMinAskValue, EthUsdMaxAskValue, EthUsdMinBidValue, EthUsdMaxBidValue);
                        EthEurCurrentAvgPrice = null;
                        EthUsdCurrentAvgPrice = marketData;

                        LastMessage = DateTime.Now;
                    }
                    if (hitBtcPair == "BTCEUR" && hitBtcExchangeWorks)
                    {
                        marketData.FirstCurrency = "BTC";
                        marketData.LastCurrency = "EUR";
                        marketData.SetTotalCoins(BtcEurMinAskValue, BtcEurMaxAskValue, BtcEurMinBidValue, BtcEurMaxBidValue);
                        BtcEurCurrentAvgPrice = null;
                        BtcEurCurrentAvgPrice = marketData;
                        LastMessage = DateTime.Now;

                    }
                    if (hitBtcPair == "ETHEUR" && hitBtcExchangeWorks)
                    {
                        marketData.FirstCurrency = "ETH";
                        marketData.LastCurrency = "EUR";
                        marketData.SetTotalCoins(EthEurMinAskValue, EthEurMaxAskValue, EthEurMinBidValue, EthEurMaxBidValue);
                        EthEurCurrentAvgPrice = null;
                        EthEurCurrentAvgPrice = marketData;
                        LastMessage = DateTime.Now;
                        Debug.WriteLine(readToEnd);

                    }
                    if (MyOwnCoins.Any())
                    {
                        if (MyOwnCoins.Select(a => a.CoinSymbol + "USD").Any(a => a == hitBtcPair) && hitBtcExchangeWorks)
                        {
                            var firstCurrency = hitBtcPair.Replace("USD", string.Empty);
                            var coin = MyOwnCoins.First(f => f.CoinSymbol == firstCurrency);
                            marketData.FirstCurrency = firstCurrency;
                            marketData.LastCurrency = "USD";
                            marketData.SetTotalCoins(coin.UsdMinAskValue, coin.UsdMaxAskValue, coin.UsdMinBidValue, coin.UsdMaxBidValue);
                            coin.CurrentUsdMarketData = null;
                            coin.CurrentUsdMarketData = marketData;

                            LastMessage = DateTime.Now;
                        }
                        if (MyOwnCoins.Select(a => a.CoinSymbol + "EUR").Any(a => a == hitBtcPair) && hitBtcExchangeWorks)
                        {
                            var firstCurrency = hitBtcPair.Replace("EUR", string.Empty);
                            var coin = MyOwnCoins.First(f => f.CoinSymbol == firstCurrency);
                            marketData.FirstCurrency = firstCurrency;
                            marketData.LastCurrency = "EUR";
                            marketData.SetTotalCoins(coin.EurMinAskValue, coin.EurMaxAskValue, coin.EurMinBidValue, coin.EurMaxBidValue);
                            coin.CurrentEurMarketData = null; coin.CurrentEurMarketData = marketData;
                            LastMessage = DateTime.Now;

                        }

                        if (MyOwnCoins.Select(a => a.CoinSymbol + "BTC").Any(a => a == hitBtcPair) && hitBtcExchangeWorks)
                        {
                            var firstCurrency = hitBtcPair.Replace("BTC", string.Empty);
                            var coin = MyOwnCoins.First(f => f.CoinSymbol == firstCurrency);
                            marketData.FirstCurrency = firstCurrency;
                            marketData.LastCurrency = "BTC";
                            marketData.SetTotalCoins(coin.BtcMinAskValue, coin.BtcMaxAskValue, coin.BtcMinBidValue, coin.BtcMaxBidValue);
                            coin.CurrentBtcMarketData = null;
                            coin.CurrentBtcMarketData = marketData;
                            LastMessage = DateTime.Now;

                        }

                        if (MyOwnCoins.Select(a => a.CoinSymbol + "ETH").Any(a => a == hitBtcPair) && hitBtcExchangeWorks)
                        {
                            var firstCurrency = hitBtcPair.Replace("ETH", string.Empty);
                            var coin = MyOwnCoins.First(f => f.CoinSymbol == firstCurrency);
                            marketData.FirstCurrency = firstCurrency;
                            marketData.LastCurrency = "ETH";
                            marketData.SetTotalCoins(coin.EthMinAskValue, coin.EthMaxAskValue, coin.EthMinBidValue, coin.EthMaxBidValue);
                            coin.CurrentEthMarketData = marketData;
                            LastMessage = DateTime.Now;

                        }

                    }
                }
                return Task.FromResult(0);

            });

            //else if (readToEnd.StartsWith($"{{\"MarketDataIncrementalRefresh\":"))
            //{
            //    var tobj = JsonConvert.DeserializeObject<MarketDataIncrementalRefreshResponse>(readToEnd);
            //    if (tobj.MarketDataIncrementalRefresh.symbol == "BTCUSD" && tobj.MarketDataIncrementalRefresh.exchangeStatus == "working")
            //    {
            //        Debug.WriteLine(readToEnd);
            //    }
            //}
            //Debug.WriteLine(readToEnd);
        }

        public ObservableCollection<MyOwnCoin> MyOwnCoins
        {
            get { return _myOwnCoins; }
            set { _myOwnCoins = value; OnPropertyChanged(); }
        }

        public MarketDataSnapshotFullRefresh EthEurCurrentAvgPrice
        {
            get { return _ethEurCurrentAvgPrice; }
            set { _ethEurCurrentAvgPrice = value; OnPropertyChanged(); }
        }

        public float EthEurMaxBidValue
        {
            get => _ethEurMaxBidValue;
            set { _ethEurMaxBidValue = value; OnPropertyChanged(); }
        }

        public float EthEurMinBidValue
        {
            get { return _ethEurMinBidValue; }
            set { _ethEurMinBidValue = value; OnPropertyChanged(); }
        }

        public float EthEurMaxAskValue
        {
            get { return _ethEurMaxAskValue; }
            set { _ethEurMaxAskValue = value;  OnPropertyChanged();}
        }

        public float EthEurMinAskValue
        {
            get { return _ethEurMinAskValue; }
            set { _ethEurMinAskValue = value; OnPropertyChanged(); }
        }

        public float BtcEurMinAskValue
        {
            get { return _btcEurMinAskValue; }
            set { _btcEurMinAskValue = value; OnPropertyChanged(); }
        }

        public float BtcEurMinBidValue
        {
            get { return _btcEurMinBidValue; }
            set { _btcEurMinBidValue = value; OnPropertyChanged(); }
        }

        public float BtcEurMaxAskValue
        {
            get { return _btcEurMaxAskValue; }
            set { _btcEurMaxAskValue = value; OnPropertyChanged(); }
        }

        public float BtcEurMaxBidValue
        {
            get { return _btcEurMaxBidValue; }
            set { _btcEurMaxBidValue = value; OnPropertyChanged();}
        }

        public MarketDataSnapshotFullRefresh BtcEurCurrentAvgPrice
        {
            get { return _btcEurCurrentAvgPrice; }
            set { _btcEurCurrentAvgPrice = value; OnPropertyChanged(); }
        }

        public WebSocket WebSocket { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}