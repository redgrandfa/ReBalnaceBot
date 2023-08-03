using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MyAlpacaStrategyLib
{

    //var account = await client.GetAccountAsync();
    //Console.WriteLine(account.ToString());

    //    //https://alpaca.markets/docs/api-references/trading-api/account/
    //    //"buying_power":200000.0,
    //    //"daytrading_buying_power":0.0,
    //    //"non_maginable_buying_power":null,
    //    //"regt_buying_power":200000.0,

    //"cash": "99614.85"

    public class AccountKeyInfo { 
        private IAccount account;
        public IAccount Account => account;


        private decimal equity;
        public decimal Equity => equity;
        public bool EnableDayTrade => equity > 25_000m;


        private IReadOnlyList<IPosition> positions;
        public IReadOnlyList<IPosition> Positions => positions;



        //禁止外部建構
        private AccountKeyInfo() { } 
        public static async Task<AccountKeyInfo> CreateAccountKeyInfoAsync(IAlpacaTradingClient client)
        {
            var cancelAllOrdersTask = client.CancelAllOrdersAsync();
            var getAccountTask = client.GetAccountAsync();
            var listPositionsTask = client.ListPositionsAsync();

            await Task.WhenAll(
                cancelAllOrdersTask,
                getAccountTask,
                listPositionsTask
            );


            var accountKeyInfo = new AccountKeyInfo();
            accountKeyInfo.account = getAccountTask.Result;
            accountKeyInfo.equity = accountKeyInfo.account.Equity.Value;

            //decimal equity = Math.Max(Account.Equity.Value, 25_000m); //可能對長期下跌做預備，只使用25萬，預先入金 避免禁止day Trade



            accountKeyInfo.positions = listPositionsTask.Result;

            return accountKeyInfo;
        }
    }
}
