using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlpacaStrategyLib
{
    public static class AssetHelpers
    {
        /// <summary>
        /// 是否可交易 (休市 停盤  封鎖特定企業)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public static async Task<bool> CheckSymbolTradable(this IAlpacaTradingClient client , string symbol)
        {
            //See If a Particular Asset is Tradable on Alpaca #
            try
            {
                var asset = await client.GetAssetAsync(symbol);
                return asset.IsTradable;
            }
            catch (Exception)
            {
                throw new Exception($"Asset not found for {symbol}.");
                //return false;
            }
        }


        //所有標的
        //var assets = await client.ListAssetsAsync(
        //    new AssetsRequest
        //    {
        //        AssetClass = AssetClass.UsEquity,
        //            //Exchange = Exchange.NyseMkt,  //assetJSON.exchange
        //        AssetStatus = AssetStatus.Active,
        //    });
        //https://docs.alpaca.markets/reference/get-v2-assets-symbol_or_asset_id

        //"class": "us_equity"
        //"exchange": "NASDAQ"
        //"symbol": "AAPL"
        //"status": "active",

        //"tradable": true,
        //"fractionable": true

        //foreach( var symbol in assets.Select(ass => ass.Symbol))
        //{
        //    Console.WriteLine(symbol);
        //}
    }


    public class PlanItemKeyInfo
    {
        public readonly string symbol;
        public readonly decimal percent;
        public readonly bool isFractionable;
        public PlanItemKeyInfo(string symbol, decimal percent, bool isFractionable)
        {
            this.symbol = symbol;
            this.percent = percent;
            this.isFractionable = isFractionable;
        }
        public PlanItemKeyInfo(string symbol, decimal percent) : this(symbol, percent, true)
        {
        }
    }
}
