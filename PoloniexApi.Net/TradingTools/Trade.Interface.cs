using System;

namespace Jojatekok.PoloniexAPI.TradingTools
{
    public interface ITrade : IOrder
    {
        DateTime Time { get; }
    }
}
