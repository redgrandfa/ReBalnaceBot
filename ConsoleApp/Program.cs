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
            //IAlpacaTradingClient client = EnvClient.GetPaperClient();
            var envClient = new EnvClient();
            var client = envClient.Client;
            var account = envClient.Account;
            //account.Equity;


            List<IAsset> a = new List<IAsset>();
            //a.Add( await client.GetAssetAsync("VT") );
            //a.Add( await client.GetAssetAsync("VTI") );
            //a.Add( await client.GetAssetAsync("VXUS") );

            //a.Add( await client.GetAssetAsync("AVUV") );

            //a.Add( await client.GetAssetAsync("DFSV") ); //Fractionable: False
            //a.Add( await client.GetAssetAsync("VIOV") );

            //a.Add( await client.GetAssetAsync("QMOM") );

            //a.Add( await client.GetAssetAsync("VFMF") );
            //a.Add( await client.GetAssetAsync("AVGE") ); //Fractionable: False

            //a.Add( await client.GetAssetAsync("AVUV") );
            a.Add( await client.GetAssetAsync("QVAL") );
            a.Add( await client.GetAssetAsync("DEEP") ); //Fractionable: False
            //a.Add( await client.GetAssetAsync("QMOM") );
            a.Add( await client.GetAssetAsync("AVDV") );
            a.Add( await client.GetAssetAsync("IVAL") );
            a.Add( await client.GetAssetAsync("FRDM") );
            a.Add( await client.GetAssetAsync("IMOM") ); //Fractionable: False

            foreach (var asset in a)
            {
                Console.WriteLine($"{asset.Symbol} Status: {asset.Status}");
                Console.WriteLine($"{asset.Symbol} IsTradable: {asset.IsTradable}");
                Console.WriteLine($"{asset.Symbol} Fractionable: {asset.Fractionable}");

                //await client.PostOrderAsync(OrderSide.Buy.Market(
                //    asset.Symbol,
                //    OrderQuantity.Notional(100m)
                //));
            }



            //await client.CancelAllOrdersAsync();

            //var order2 = await client.PostOrderAsync(OrderSide.Buy.Market(
            //    "AAPL",
            //    1
            //));

            //var order3 = await client.PostOrderAsync(OrderSide.Buy.Market(
            //    "AAPL",
            //    OrderQuantity.Notional(1111m)
            //));


            //var a = new ReBalanceOperation(client, account);
            //a.TryReBalanceAll();


            Console.ReadLine();
        }
    }
}