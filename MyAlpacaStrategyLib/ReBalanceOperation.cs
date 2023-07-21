using Alpaca.Markets;
using MyAlpacaStrategyLib;

namespace MyAlpacaStrategyLib
{
    public class ReBalanceOperation
    {
        private static readonly IAlpacaTradingClient client;
        private static readonly Dictionary<string, PlanItemKeyInfo> plan;
        static ReBalanceOperation()
        {
            client = EnvClient.GetClient();
            plan = new Dictionary<string, PlanItemKeyInfo> //100% 計畫   //TODO 必須先賣後買?  //buying power > equity
            {
                {"AVUV" ,  new PlanItemKeyInfo("AVUV" , 0.100m) },
                {"QVAL" ,  new PlanItemKeyInfo("QVAL" , 0.115m) },
                {"DEEP" ,  new PlanItemKeyInfo("DEEP" , 0.115m , false) },
                {"QMOM" ,  new PlanItemKeyInfo("QMOM" , 0.220m) },
                {"AVDV" ,  new PlanItemKeyInfo("AVDV" , 0.080m) },
                {"IVAL" ,  new PlanItemKeyInfo("IVAL" , 0.095m) },
                {"FRDM" ,  new PlanItemKeyInfo("FRDM" , 0.095m) },
                {"IMOM" ,  new PlanItemKeyInfo("IMOM" , 0.180m  , false) }, 
            };
        }

        private IAccount Account { get; set; }

        //private 完美賣出額
        //private 完美買入額

        public ReBalanceOperation() 
        {
        }


        public async Task TryReBalanceAll() {
            var cancelAllOrdersTask = client.CancelAllOrdersAsync();
            var getAccountTask = client.GetAccountAsync();
            var listPositionsTask = client.ListPositionsAsync();

            await Task.WhenAll(getAccountTask, cancelAllOrdersTask, listPositionsTask);

            Account = getAccountTask.Result;
            var allPositions = listPositionsTask.Result;



            //每個 ReBalance項目，所需的部位資訊
            var positionsToReBalance = new List<PositionKeyOnfo>();
            //需要檢查再平衡的 =  計劃書 U 部位


            foreach (var position in allPositions)
            {
                positionsToReBalance.Add(position.GetIPositionKeyInfo());
            }

            // 不在目前部位裡 / 被更新的計畫納入  必然為買
            foreach (var planItem in plan)
            {
                var positionFind = positionsToReBalance.FirstOrDefault(p => p.symbol == planItem.Key);
                if ( positionFind != null )
                {
                    positionFind.isFractionable = planItem.Value.isFractionable;
                }
                else
                {
                    var p = new PositionKeyOnfo(planItem.Key);
                    positionsToReBalance.Add( p ); //not yet know Fractionable
                }
            }



            foreach (var positionKeyInfo in positionsToReBalance) { 
                //TODO 是否有必要檢查部位可交易
                bool isTradable = await client.CheckSymbolTradable(positionKeyInfo.symbol);

                if (isTradable) {
                    var idealMarketValue = 0m;
                    if ( plan.ContainsKey(positionKeyInfo.symbol) )
                        idealMarketValue = Account.Equity.Value * plan[positionKeyInfo.symbol].percent;

                    await ReBalancePosition(positionKeyInfo, idealMarketValue );
                }
                else
                {
                    Console.WriteLine($"{positionKeyInfo.symbol} 不可交易");
                }
            }
        }

        public static async Task ReBalancePosition(PositionKeyOnfo positionKeyInfo , decimal idealMarketValue )
        {
            decimal? marketValue = positionKeyInfo.marketValue;
            if (marketValue == null) {
                Console.WriteLine("有倉，取不到市值");
                return;
            }
            decimal diffMarketValue = idealMarketValue - (decimal)marketValue;


            OrderSide side = OrderSide.Buy;
            OrderQuantity orderQuantity;

            if (diffMarketValue < 0m)
            {
                side = OrderSide.Sell;
            }

            string symbol = positionKeyInfo.symbol;

            bool isFractionable = positionKeyInfo.isFractionable ?? (await client.GetAssetAsync(symbol)).Fractionable;
            //"Notional"是金融術語，指的是某個金融衍生品（例如期貨合約、選擇權合約等）的標的資產的價值，而不是實際投入的資金數額
            if (isFractionable) // 以錢下單， 一元為最小單位，自動拆
            {
                //limit price increment must be > 0.1
                //RestClientErrorException: notional amount must be >= 1.00
                //多買 少賣
                //var diffNotion = Math.Round(diffMarketValue, 1, MidpointRounding.ToPositiveInfinity);  
                var diffNotion = Math.Ceiling(diffMarketValue);  
                orderQuantity = OrderQuantity.Notional(Math.Abs(diffNotion));
            }
            else // 股票須整數量
            {
                //無單價 需取得市場現價，乾脆先買一單位  
                //decimal unitPrice = positionKeyInfo.unitPrice ?? diffMarketValue; 
                if (positionKeyInfo.unitPrice.HasValue)
                {
                    decimal unitPrice = positionKeyInfo.unitPrice.Value; 
                    var diffQty = Math.Ceiling(diffMarketValue / unitPrice);
                    orderQuantity = OrderQuantity.FromInt64(Math.Abs((long)diffQty));
                }
                else
                {
                    orderQuantity = OrderQuantity.FromInt64(1L);
                }
            }

            if (orderQuantity.Value <= 0) {
                Console.WriteLine($"{symbol}: balance");
                return;
            }

            try
            {
                string unitText = orderQuantity.IsInDollars ? "dollars" : "shares";
                Console.WriteLine($"\t{symbol}: Try {side.ToString()} {orderQuantity.Value} {unitText}");
                await client.PostOrderAsync(side.Market(
                    symbol,
                    orderQuantity
                ));

                Console.WriteLine($"\t\tdone");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
