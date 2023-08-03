using Alpaca.Markets;

namespace MyAlpacaStrategyLib
{

    //"symbol":"BTC/USD",
    //"qty":1.0,
    //"current_price":20077.0,

    //"side":"long",

    //IReadOnlyList<IPosition> positions = await client.ListPositionsAsync();
    //IPosition symbolPosition = await client.GetPositionAsync(symbol);


    /// <summary>
    /// 無倉 => 市值 0m，單價null
    /// 有倉 => 市值 decimal? ，單價decimal?
    /// </summary>
    public class PositionKeyInfo
    {
        public readonly string symbol;
        public readonly decimal qty;
        public readonly decimal? unitPrice;
        public readonly decimal? marketValue;
        public bool? isFractionable = null;

        public PositionKeyInfo(string symbol, decimal qty, decimal? unitPrice, decimal? marketValue)
        {
            this.symbol = symbol;
            this.qty = qty;
            this.unitPrice = unitPrice;
            this.marketValue = marketValue;
        }

        /// <summary>
        /// 目前無持倉的假部位
        /// </summary>
        public PositionKeyInfo(string symbol):this(symbol, 0m, null, 0m) {  }
    }

    public static class PositionHelpers
    {
        /// <summary>
        /// 還不知道是否可碎
        /// </summary>
        public static PositionKeyInfo GetIPositionKeyInfo(this IPosition position)
        {
            return new PositionKeyInfo(
                position.Symbol,
                position.Quantity,
                position.AssetCurrentPrice,
                position.MarketValue
            );
        }

    }
}