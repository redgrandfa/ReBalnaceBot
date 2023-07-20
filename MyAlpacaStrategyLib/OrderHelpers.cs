using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlpacaStrategyLib
{

    //台灣視角 UTC+8 21:30 ~ 04:00   
    //預市交易（Pre-Market Trading）：     時間：美東時間（Eastern Time，ET） 04:00  ~ 09:30  UTC-4
    //正式開盤交易（Regular Trading Hours）：時間：美東時間（Eastern Time，ET）09:30 ~ 16:00  UTC-4

    //三月中旬，時鐘會向前調整一小時，時間差會變為 UTC-4 小時。
    //而在夏令時間結束，即在每年的十一月初，時鐘會向後調整一小時，時間差又恢復為 UTC-5 小時。

    //=> UTC+0  08:00 ~ 13:30 ~ 20:00



    //只有兩方向
    //OrderSide.Buy
    //OrderSide.Sell
    //成交後，postion不會馬上變

    //An order request may be rejected if the account is not authorized for trading, or  帳戶無交易權
    //    if the tradable balance is insufficient to fill the order.. 結餘不夠

    //https://alpaca.markets/docs/api-references/trading-api/orders/#order-entity

    // "filled_at": null,
    // "expired_at": null, //可設定期限?
    // "canceled_at": null,
    // "failed_at": null,

    // "symbol": "AAPL",
    // "qty": null,
    // "filled_qty": "0",
    // "filled_avg_price": null,

    // "order_type": "market",
    // "type": "market",
    // "limit_price": null,
    // "stop_price": null,
    // "status": "accepted",

    public static class OrderHelpers
    {

        public static async Task<IOrder?> PlaceMarketBuyOrder(this IAlpacaTradingClient client , String symbol , Int64 quantity)
        {
            var order = await client.PostOrderAsync(OrderSide.Buy.Market(symbol, quantity));

            Console.WriteLine(order);
            return order;
            //invalid crypto time_in_force
        }

        //限價買入 IOC? 五分鐘?
        public static async Task<IOrder?> PlaceLimitedBuyOrder(this IAlpacaTradingClient client, String symbol, Int64 quantity)
        {
            var order = await client.PostOrderAsync(OrderSide.Buy.Market(symbol, quantity));

            Console.WriteLine(order);
            return order;
        }

        //限價賣出 IOC? 五分鐘?
        public static async Task<IOrder?> PlaceLimitedSellOrder(this IAlpacaTradingClient client, String symbol, Int64 quantity)
        {
            var order = await client.PostOrderAsync(OrderSide.Sell.Market(symbol, quantity));

            Console.WriteLine(order);


            //需要判斷是否 超持有量

            return order;
        }

        //order = await client.PostOrderAsync(
        //        LimitOrder.Sell("AMD", 1, 20.50M).WithDuration(TimeInForce.Opg));

        //===查所有 進行中的訂單
        //https://alpaca.markets/docs/trading/getting_started/how-to-orders/#retrieve-all-orders
        //===查特定 進行中的訂單

        //===刪 進行中的訂單


    }
}
