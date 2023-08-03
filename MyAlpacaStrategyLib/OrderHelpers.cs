using Alpaca.Markets;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlpacaStrategyLib
{
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
        public static int ToQuantitySign (this OrderSide side)
        {
            return side == OrderSide.Buy ? 1 : -1;
        }

        //public static LimitOrderKeyInfo ToLimitOrderKeyInfo(this IOrder order)
        //{
        //    return new LimitOrderKeyInfo
        //    (
        //        order.OrderId,
        //        order.OrderSide,
        //        order.Quantity.Value,
        //        order.LimitPrice.Value
        //    );
        //}
    }

    //public class LimitOrderKeyInfo
    //{
    //    public Guid orderId;
    //    public OrderSide side;
    //    public decimal qty;
    //    public decimal limitPrice;

    //    public LimitOrderKeyInfo(Guid orderId, OrderSide side, decimal qty, decimal limitPrice)
    //    {
    //        this.orderId = orderId;
    //        this.side = side;
    //        this.qty = qty;
    //        this.limitPrice = limitPrice;
    //    }
    //}
}
