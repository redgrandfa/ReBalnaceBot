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
        private const String KEY_ID = "PK8IZKEPBUY7KKLCFPFM";
        private const String SECRET_KEY = "u3JLg1v8pgNu6VXa6clnIyfjRugh25qSma61diz0";
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

        public IAlpacaTradingClient Client { get; }
        public IAccount Account { get; }
        public EnvClient()
        {
            Client = GetPaperClient() ;
            //Client = GetLiveClient() ;
            Account = Client.GetAccountAsync().GetAwaiter().GetResult();
        }

        public async Task Test()
        {
            //client.CancelAllOrdersAsync();
            //client.CancelOrderAsync(new Guid());

            //client.DeleteAllPositionsAsync();

            //client.PatchOrderAsync


        }
    }
}
