namespace HitBtcOrchestrator
{
    public class MarketDataIncrementalRefresh
    {
        public int seqNo { get; set; }
        public long timestamp { get; set; }
        public string symbol { get; set; }
        public string exchangeStatus { get; set; }
        public object[] ask { get; set; }
        public Bid[] bid { get; set; }
        public Trade[] trade { get; set; }
    }
}