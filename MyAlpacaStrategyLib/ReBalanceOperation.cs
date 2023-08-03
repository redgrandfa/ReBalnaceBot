using Alpaca.Markets;
using MyAlpacaStrategyLib;
using System.Drawing;
using System.Text;

namespace MyAlpacaStrategyLib
{
    public class ReBalanceOperation
    {
        private static decimal SingleOrderAmountLowerBound = -218.34m; //218.34061135
        //private decimal 完美買入額
        private static readonly IAlpacaTradingClient client;

        private static readonly Dictionary<string, PlanItemKeyInfo> plan;
        //當天 一商品可以一直做同一方向，但不可兩向
        private static Dictionary<string, OrderSide?> todayPositionSides;

        private static bool EnableDayTrade = false;

        static ReBalanceOperation()
        {
            client = EnvClient.GetClient();
            plan = new() //100% 計畫   //buying power不足的情境下 必須先賣後買?
            {
                {"AVUV" ,  new PlanItemKeyInfo("AVUV" , 0.100m) },
                {"QVAL" ,  new PlanItemKeyInfo("QVAL" , 0.115m) },
                {"DEEP" ,  new PlanItemKeyInfo("DEEP" , 0.115m , false) },
                {"QMOM" ,  new PlanItemKeyInfo("QMOM" , 0.220m) },
                {"AVDV" ,  new PlanItemKeyInfo("AVDV" , 0.080m) },
                {"IVAL" ,  new PlanItemKeyInfo("IVAL" , 0.095m) },
                {"FRDM" ,  new PlanItemKeyInfo("FRDM" , 0.095m) },
                {"IMOM" ,  new PlanItemKeyInfo("IMOM" , 0.180m , false) }, 
            };

            todayPositionSides = new()
            {
                {"AVUV" ,  null },
                {"QVAL" ,  null },
                {"DEEP" ,  null },
                {"QMOM" ,  null },
                {"AVDV" ,  null },
                {"IVAL" ,  null },
                {"FRDM" ,  null },
                {"IMOM" ,  null },
            };
        }

        //App 必須在當日第一次成交前部署， 不然會 成交過的Side 會被重置
        public static void ResetTodayPositionSides()
        {
            foreach (var positionSide in todayPositionSides)
            {
                //positionSidePair.Value = null;
                todayPositionSides[positionSide.Key] = null;
            }
        }


        private readonly IMemoryCacheRepository _cacheRepo;
        private readonly Logger _logger;

        private AccountKeyInfo AccountKeyInfo;

        public ReBalanceOperation(IMemoryCacheRepository cacheRepo, Logger logger)
        {
            _cacheRepo = cacheRepo;
            _logger = logger;
        }


        /// <summary>
        /// 各部位，限價單平衡一輪，不一定會成交
        /// </summary>
        public async Task TryReBalanceAll() {
            AccountKeyInfo = await AccountKeyInfo.CreateAccountKeyInfoAsync(client);

            //每個 ReBalance項目，所需的部位資訊
            var positionsToReBalance = new List<PositionKeyInfo>();
            //需要檢查再平衡的 =  計劃書 U 部位


            foreach (var position in AccountKeyInfo.Positions)
            {
                positionsToReBalance.Add(position.GetIPositionKeyInfo());
            }

            // 不在目前部位裡
            foreach (var positionSide in todayPositionSides)
            {
                var symbol = positionSide.Key;
                var planItem = plan[symbol];

                var positionFind = positionsToReBalance.FirstOrDefault(p => p.symbol == symbol);
                if ( positionFind != null )
                {
                    positionFind.isFractionable = planItem.isFractionable;
                }
                else
                {
                    var p = new PositionKeyInfo(symbol);
                    positionsToReBalance.Add( p ); //not yet know Fractionable
                }
            }



            foreach (var positionKeyInfo in positionsToReBalance) { 
                StringBuilder sb = new StringBuilder();

                //TODO 是否有必要檢查部位可交易
                bool isTradable = await client.CheckSymbolTradable(positionKeyInfo.symbol);
                if (isTradable) {
                    var idealMarketValue = 0m;
                    if ( plan.ContainsKey(positionKeyInfo.symbol) )
                        idealMarketValue = AccountKeyInfo.Equity * plan[positionKeyInfo.symbol].percent;

                    await ReBalanceAPosition(positionKeyInfo, idealMarketValue , sb);
                }
                else
                {
                    var msg = $"{positionKeyInfo.symbol} 不可交易";
                    sb.AppendLine(msg);
                }

                //寫入資料庫
                _logger.Log(sb.ToString());
            }
        }

        public async Task ReBalanceAPosition(PositionKeyInfo positionKeyInfo , decimal idealMarketValue , StringBuilder sb)
        {
            string symbol = positionKeyInfo.symbol;
             sb.AppendLine($"{symbol} Orders:");

            decimal? marketValue = positionKeyInfo.marketValue;
            if (marketValue == null) {
                 sb.AppendLine("有倉，取不到市值");
                return;
            }
            decimal diffMarketValue = idealMarketValue - (decimal)marketValue;
            if ( diffMarketValue == 0 )
            {
                sb.AppendLine($"{nameof(diffMarketValue) } == 0");
                return;
            }

            // 【DayTrade - Check Direction】不符合 此部位 今天能做的方向
            //Func<string, decimal, bool> FilteredBySide = (symbol, diffMarketValue) =>
            //{
            //    return (
            //        || (diffMarketValue > 0 && todayPositionSides[symbol] == OrderSide.Sell)
            //        || (diffMarketValue < 0 && todayPositionSides[symbol] == OrderSide.Buy)
            //    );
            //};
            //EnableDayTrade &&
            if (  (
                (diffMarketValue > 0 && todayPositionSides[symbol] == OrderSide.Sell) ||
                (diffMarketValue < 0 && todayPositionSides[symbol] == OrderSide.Buy)
             )) 
            {
                sb.AppendLine("not today direction");
                return;
            }

            //bool isFractionable = positionKeyInfo.isFractionable ?? (await client.GetAssetAsync(symbol)).Fractionable;
            //"Notional"是金融術語，指的是某個金融衍生品（例如期貨合約、選擇權合約等）的標的資產的價值，而不是實際投入的資金數額
            //if (isFractionable) // 以錢下單， 一元為最小單位，自動拆
            //{
            //    //Both notional and qty fields can take up to 9 decimal point values => 以單價算出數量 ，進位到7位小數?
            //    //Day trading fractional shares counts towards your day trade count
            //    //can only be bought or sold with market orders during normal market hours

            //    //買多 賣少
            //    //limit price increment must be > 0.1
            //    var diffNotion = Math.Round(diffMarketValue, 1, MidpointRounding.ToPositiveInfinity);

            //    //RestClientErrorException: notional amount must be >= 1.00
            //    //var diffNotion = Math.Ceiling(diffMarketValue);


            //    diffNotion = Math.Abs(diffNotion);
            //    if( diffNotion < 1.00m && diffNotion > 0m )
            //    {
            //        diffNotion = 1.00m;
            //    }

            //    orderQuantity = OrderQuantity.Notional(diffNotion);
            //}
            //else // 股票須整數量
            //{

            decimal unitPrice = 0m;
            decimal diffQty = 0m; 

            if ( positionKeyInfo.unitPrice.HasValue )
            {
                unitPrice = positionKeyInfo.unitPrice.Value;
            }
            else
            //無單價 需取得市場現價，乾脆只先買一單位  
            {
                var p = (await client.PostOrderAsync(OrderSide.Buy.Market(symbol, 1L)) ).AverageFillPrice;
                while (p.HasValue)
                {
                    unitPrice = p.Value; 
                }
                diffQty -= 1m;
            }

            //單價必須兩位小數。
            if(diffMarketValue >= 0) //買 單價要+ ，量-
            {
                unitPrice = Math.Round(unitPrice
                    , 2, MidpointRounding.ToPositiveInfinity);
            }
            else //賣 單價要- ，量+
            {
                unitPrice = Math.Round(unitPrice
                    , 2, MidpointRounding.ToNegativeInfinity);
            }

            diffQty += Math.Ceiling(diffMarketValue / unitPrice);

            //}

            //TODO: 減少刪單? 紀錄qty和 uniprice ，比較 舊的 訂單qty和 uniprice ：
            // 拆單情境很麻煩  每一單可能部分成交

            //Guid orderId = null;
            //IOrder oo = await client.GetOrderAsync(orderId);
            //var unclosedQty = (oo.Quantity - oo.FilledQuantity) * oo.OrderSide.ToQuantitySign();
            //if (diffQty == unclosedQty && unitPrice == oo.LimitPrice)
            //{
            //    sb.AppendLine($"As old order: {diffQty} shares , price = {unitPrice}");
            //    return;
            //}
                //await client.CancelOrderAsync(orderId);

            try
            {
                if(diffQty > 0m)
                {
                    var side = OrderSide.Buy;
                    long q = (long)diffQty;
                    var o = await client.PostOrderAsync(side.Limit(
                        symbol,
                        q,
                        unitPrice
                    ));
                    sb.AppendLine($"\t post order - buy {q} shares , price = {unitPrice}");

                    // 【DayTrade - Record Direction】 //沒成交...也記錄?....
                    RecordTodaySide(symbol, side);
                }
                else if(diffQty < 0m)
                {
                    var side = OrderSide.Sell;
                    decimal singleOrderQty_lowerBound = Math.Ceiling(SingleOrderAmountLowerBound / unitPrice);

                    while (diffQty < 0m)
                    {
                        var tempQ = diffQty;
                        if(diffQty < singleOrderQty_lowerBound) //若會超賣
                        {
                            tempQ = singleOrderQty_lowerBound;
                        }
                        long q = -(long)tempQ;

                        await client.PostOrderAsync(side.Limit(
                            symbol,
                            q,
                            unitPrice
                        ));

                        diffQty -= singleOrderQty_lowerBound;
                         sb.AppendLine($"\t post order - sell {q} shares , price = {unitPrice}");
                    }

                    RecordTodaySide(symbol, side);
                }
            }
            catch (Exception ex)
            {
                 sb.AppendLine(ex.Message);
            }
        }


        public void RecordTodaySide (string symbol, OrderSide side)
        {
            if (todayPositionSides[symbol] == null)
            {
                todayPositionSides[symbol] = side;
            }
        }
    }
}
