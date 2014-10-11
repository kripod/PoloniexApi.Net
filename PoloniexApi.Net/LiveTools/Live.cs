using Jojatekok.PoloniexAPI.MarketTools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WampSharp.V2;

namespace Jojatekok.PoloniexAPI.LiveTools
{
    public class Live : ILive
    {
        private const string SubjectNameTicker = "ticker";
        private const string SubjectNameTrollbox = "trollbox";

        public event EventHandler<TickerChangedEventArgs> OnTickerChanged;
        public event EventHandler<TrollboxMessageEventArgs> OnTrollboxMessage;

        private IWampChannel WampChannel { get; set; }
        private Task WampChannelOpenTask { get; set; }

        private readonly IDictionary<string, IDisposable> _activeSubscriptions = new Dictionary<string, IDisposable>();
        private IDictionary<string, IDisposable> ActiveSubscriptions {
            get { return _activeSubscriptions; }
        }

        private readonly ObservableDictionary<CurrencyPair, MarketData> _tickers = new ObservableDictionary<CurrencyPair, MarketData>();
        public ObservableDictionary<CurrencyPair, MarketData> Tickers {
            get { return _tickers; }
        }

        internal Live()
        {
            WampChannel = new DefaultWampChannelFactory().CreateJsonChannel(Helper.ApiUrlWssBase, "realm1");
            WampChannelOpenTask = WampChannel.Open();
        }

        public async Task SubscribeToTickerAsync()
        {
            if (!ActiveSubscriptions.ContainsKey(SubjectNameTicker)) {
                await WampChannelOpenTask;
                ActiveSubscriptions.Add(SubjectNameTicker, WampChannel.RealmProxy.Services.GetSubject(SubjectNameTicker).Subscribe(x => ProcessMessageTicker(x.Arguments)));
            }
        }

        public async Task SubscribeToTrollboxAsync()
        {
            if (!ActiveSubscriptions.ContainsKey(SubjectNameTrollbox)) {
                await WampChannelOpenTask;
                ActiveSubscriptions.Add(SubjectNameTrollbox, WampChannel.RealmProxy.Services.GetSubject(SubjectNameTrollbox).Subscribe(x => ProcessMessageTrollbox(x.Arguments)));
            }
        }

        private void ProcessMessageTicker(ISerializedValue[] arguments)
        {
            var currencyPair = arguments[0].Deserialize<string>().ToCurrencyPair();
            var priceLast = arguments[1].Deserialize<double>();
            var orderTopSell = arguments[2].Deserialize<double>();
            var orderTopBuy = arguments[3].Deserialize<double>();
            var priceChangePercentage = arguments[4].Deserialize<double>();
            var volume24HourBase = arguments[5].Deserialize<double>();
            var volume24HourQuote = arguments[6].Deserialize<double>();
            var isFrozenInternal = arguments[7].Deserialize<byte>();

            var marketData = new MarketData {
                PriceLast = priceLast,
                OrderTopSell = orderTopSell,
                OrderTopBuy = orderTopBuy,
                PriceChangePercentage = priceChangePercentage,
                Volume24HourBase = volume24HourBase,
                Volume24HourQuote = volume24HourQuote,
                IsFrozenInternal = isFrozenInternal
            };

            if (Tickers.ContainsKey(currencyPair)) {
                Tickers[currencyPair] = marketData;
            } else {
                Tickers.Add(currencyPair, marketData);
            }

            if (OnTickerChanged != null) OnTickerChanged(this, new TickerChangedEventArgs(currencyPair, marketData));
        }

        private void ProcessMessageTrollbox(ISerializedValue[] arguments)
        {
            if (OnTrollboxMessage == null) return;

            var messageType = arguments[0].Deserialize<string>();
            var messageNumber = arguments[1].Deserialize<ulong>();
            var senderName = arguments[2].Deserialize<string>();
            var messageText = arguments[3].Deserialize<string>();
            var senderReputation = arguments[4].Deserialize<uint>();

            OnTrollboxMessage(this, new TrollboxMessageEventArgs(senderName, senderReputation, messageType, messageNumber, messageText));
        }
    }
}
