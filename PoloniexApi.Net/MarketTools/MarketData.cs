using Newtonsoft.Json;

namespace Jojatekok.PoloniexAPI.MarketTools
{
    public class MarketData : IMarketData
    {
        [JsonProperty("last")]
        public double PriceLast { get; private set; }
        [JsonProperty("percentChange")]
        public double PriceChangePercentage { get; private set; }

        [JsonProperty("baseVolume")]
        public double Volume24HourBase { get; private set; }
        [JsonProperty("quoteVolume")]
        public double Volume24HourQuote { get; private set; }

        [JsonProperty("highestBid")]
        public double OrderTopBuy { get; private set; }
        [JsonProperty("lowestAsk")]
        public double OrderTopSell { get; private set; }
        public double OrderSpread {
            get { return (OrderTopSell - OrderTopBuy).Normalize(); }
        }
        public double OrderSpreadPercentage {
            get { return OrderTopSell / OrderTopBuy - 1; }
        }
    }
}
