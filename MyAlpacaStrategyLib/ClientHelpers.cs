using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlpacaStrategyLib
{
    public class EnvClient
    {
        private const String KEY_ID = "PKJKRDUCZFXTYP4PZNHG";
        private const String SECRET_KEY = "ThXy991piqh2GaC3mfkXKMgXOy3NRkULrKRLdVeE";

        internal static IAlpacaTradingClient GetClient()
        {
            return GetPaperClient();
        }

        public static IAlpacaTradingClient GetPaperClient()
        {
            return Environments.Paper
                .GetAlpacaTradingClient(new SecretKey(KEY_ID, SECRET_KEY));
        }

        //TODO check KEY_ID, SECRET_KEY for live
        public static IAlpacaTradingClient GetLiveClient()
        {
            return Environments.Live
                .GetAlpacaTradingClient(new SecretKey(KEY_ID, SECRET_KEY));
        }
        //============================================

        public async Task Test()
        {
            //Client.GetAccountAsync()
            //client.CancelAllOrdersAsync();
            //client.CancelOrderAsync(new Guid());

            //client.DeleteAllPositionsAsync();

            //client.PatchOrderAsync
        }
    }
}
