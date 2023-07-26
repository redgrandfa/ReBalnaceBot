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
        private const string KEY_ID = "PKULL0A9EEBUV39VEH9Q";
        private const string SECRET_KEY = "3QWqxPupX7T5qcebZGpZEKBEnWRyBDogj8YQA3CK";

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
    }
}
