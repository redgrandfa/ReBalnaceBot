//dotnet add package Alpaca.Markets
using System;
using Alpaca.Markets;
using System.Threading.Tasks;
using MyAlpacaStrategyLib;

namespace AlpacaExample
{
    //internal sealed class Program
    internal static class Program
    {
        // 可能需要 appservice置換 環境變數

        public static async Task Main()
        {
            IAlpacaTradingClient client = EnvClient.GetPaperClient();



            var account = await client.GetAccountAsync();
            List<IAsset> assets = new List<IAsset>()
            {
                //await client.GetAssetAsync("VT") ,
                //await client.GetAssetAsync("VTI") ,
                //await client.GetAssetAsync("VXUS") ,

                //await client.GetAssetAsync("AVUV") ,

                //await client.GetAssetAsync("DFSV") , //Fractionable: False
                //await client.GetAssetAsync("VIOV") ,

                //await client.GetAssetAsync("QMOM") ,

                //await client.GetAssetAsync("VFMF") ,
                //await client.GetAssetAsync("AVGE") , //Fractionable: False

                await client.GetAssetAsync("AVUV") ,
                await client.GetAssetAsync("QVAL") ,
                await client.GetAssetAsync("DEEP") , //Fractionable: False
                await client.GetAssetAsync("QMOM") ,
                await client.GetAssetAsync("AVDV") ,
                await client.GetAssetAsync("IVAL") ,
                await client.GetAssetAsync("FRDM") ,
                await client.GetAssetAsync("IMOM") , //Fractionable: False
            };

            foreach (var asset in assets)
            {
                //Console.WriteLine($"{asset.Symbol} Status: {asset.Status}");
                //Console.WriteLine($"{asset.Symbol} IsTradable: {asset.IsTradable}");
                //Console.WriteLine($"{asset.Symbol} Fractionable: {asset.Fractionable}");
                await client.PostOrderAsync(OrderSide.Buy.Market(
                    asset.Symbol,
                    1L
                ));
            }


            // OK

            //var c = await client.ListOrdersAsync();

            //var a = new ReBalanceOperation();
            //await a.TryReBalanceAll();


            //Client.GetAccountAsync()
            //client.CancelAllOrdersAsync();
            //client.CancelOrderAsync(new Guid());

            //client.DeleteAllPositionsAsync();

            //client.PatchOrderAsync()
            //var id = (await client.PostOrderAsync(OrderSide.Buy.Market(
            //    "q",
            //    1L
            //))).OrderId;

            //await client.PostOrderAsync(OrderSide.Buy.Limit(
            //    "DEEP",
            //    1L,
            //    35.5m
            //));

            Console.ReadLine();
        }
    }
}