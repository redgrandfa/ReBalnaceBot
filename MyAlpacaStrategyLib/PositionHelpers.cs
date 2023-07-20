using Alpaca.Markets;

namespace MyAlpacaStrategyLib
{

    //"symbol":"BTC/USD",
    //"qty":1.0,
    //"current_price":20077.0,

    //"side":"long",
    public class PositionKeyOnfo
    {
        public readonly string symbol;
        public readonly decimal qty;
        public readonly decimal? unitPrice;
        public readonly decimal? marketValue;
        public readonly bool isFractionable =true;

        public PositionKeyOnfo(string symbol, decimal qty, decimal unitPrice, decimal marketValue)
        {
            this.symbol = symbol;
            this.qty = qty;
            this.unitPrice = unitPrice;
            this.marketValue = marketValue;
        }

        public PositionKeyOnfo(string symbol):this(symbol, 0, 0, 0) //目前無持倉
        {
        }
    }

    public static class PositionHelpers
    {
        public static PositionKeyOnfo GetIPositionKeyInfo(this IPosition position)
        {
            return new PositionKeyOnfo(
                position.Symbol,
                position.Quantity,
                (decimal)position.AssetCurrentPrice,
                (decimal)position.MarketValue
            );
        }

        //IReadOnlyList<IPosition> positions = await client.ListPositionsAsync();
        //IPosition symbolPosition = await client.GetPositionAsync(symbol);
    }


    //public Position
}