using Alpaca.Markets;
using MyAlpacaStrategyLib;

namespace MyAlpacaStrategyLib
{
    public class AssetKeyInfo
    {
        public readonly string symbol;
        public readonly decimal percent;
        public readonly bool isFractionable;
        public AssetKeyInfo(string symbol, decimal percent, bool isFractionable)
        {
            this.symbol = symbol;
            this.percent = percent;
            this.isFractionable = isFractionable;
        }
        public AssetKeyInfo(string symbol, decimal percent): this(symbol, percent , true)
        {
        }

    }
    public class ReBalanceOperation
    {
        //現況
        private readonly IAlpacaTradingClient client;
        private readonly IAccount account;
        private readonly decimal additionalHypotheticalCash; //從DB取'額外未入假想金
        private decimal CurrentCash => this.account.TradableCash + additionalHypotheticalCash;
        // buypower


        //理想
        private readonly Dictionary<string, AssetKeyInfo> plan;

        //private 完美賣出額
        //private 完美買入額

        public ReBalanceOperation(IAlpacaTradingClient client , IAccount account) 
            : this(client , account, 
                 new Dictionary<string, AssetKeyInfo>
                 {
                    {nameof(CurrentCash) , new AssetKeyInfo(nameof(CurrentCash) ,0m , false) },
                    {"AVUV" ,  new AssetKeyInfo("AVUV" , 0m) },
                    {"QVAL" ,  new AssetKeyInfo("QVAL" , 0m) },
                    {"DEEP" ,  new AssetKeyInfo("DEEP" , 0m , false) },
                    {"QMOM" ,  new AssetKeyInfo("QMOM" , 0m) },
                    {"AVDV" ,  new AssetKeyInfo("AVDV" , 0m) },
                    {"IVAL" ,  new AssetKeyInfo("IVAL" , 0m) },
                    {"FRDM" ,  new AssetKeyInfo("FRDM" , 0m) },
                    {"IMOM" ,  new AssetKeyInfo("IMOM" , 0m , false) },
                 }
            )
        {
        }

        public ReBalanceOperation(IAlpacaTradingClient client, IAccount account, Dictionary<string, AssetKeyInfo> plan)
        {
            this.client = client;
            this.account = account;

            this.plan = plan;

            additionalHypotheticalCash = 0;
        }

        public async Task TryReBalanceAll() {
            //刪所有 open中的訂單 
            //await client.CancelAllOrdersAsync();
            //所有部位物件
            var allPositions = await client.ListPositionsAsync();

            //每個 ReBalance項目，所需的部位資訊
            var positionsToReBalance = new List<PositionKeyOnfo>(); 
            //不再計劃書李 / 不在部位裡


            //var 可取得市值的資產總市值 = CurrentCash ;

            var 可取得市值的資產的總比例 = plan[nameof(CurrentCash)].percent;


            foreach (var position in allPositions)
            {
                if ( position.MarketValue.HasValue) //什麼情況拿不到?
                {
                    positionsToReBalance.Add(position.GetIPositionKeyInfo());

                    if (plan.ContainsKey(position.Symbol))
                    {
                        可取得市值的資產的總比例 += plan[position.Symbol].percent; 
                    }
                    else  //不在計劃書裡 / 被更新的計畫除名
                    {
                        可取得市值的資產的總比例 += 0;
                    }
                }
                else
                {
                    Console.WriteLine($"{position.Symbol}無MarketValue");
                }
            }

            // 不在目前部位裡 / 被更新的計畫納入





            foreach (var positionKeyInfo in positionsToReBalance) { 
                //TODO 是否有必要檢查部位可交易
                bool isAvalible = await client.CheckSymbolTrable(positionKeyInfo.symbol);
                //if (!isAvalible) {
                //    continue;
                //}
                if (isAvalible) {
                    await ReBalancePosition(positionKeyInfo, 可取得市值的資產總市值 , 可取得市值的資產的總比例);
                    Console.WriteLine($"ReBalancePosition {positionKeyInfo.symbol} done");
                }
                else
                {
                    Console.WriteLine($"{positionKeyInfo.symbol}無MarketValue");
                }
            }
        }

        //可取得市值的資產們 的總市值?
        public async Task ReBalancePosition(PositionKeyOnfo positionKeyInfo, decimal 可取得市值的資產總市值 , decimal 可取得市值的資產的總比例)
        {
            string symbol = positionKeyInfo.symbol ;
            decimal qty = positionKeyInfo.qty ;
            decimal unitPrice = positionKeyInfo.unitPrice.Value; // 一定有市值 才會被ADD進LIST
            decimal marketValue = positionKeyInfo.marketValue.Value; // 一定有市值 才會被ADD進LIST


            var 理應marketValue = 可取得市值的資產總市值 * plan[symbol].percent / 可取得市值的資產的總比例;

            // 理應加減碼
            var diffMarketValue = 理應marketValue - marketValue;

            if (diffMarketValue == 0m)
            {
                return;
            }

            OrderSide side = OrderSide.Buy;
            OrderQuantity orderQuantity;

            //Func<decimal, decimal> sign = (x) => x;

            if (diffMarketValue < 0m)
            {
                side = OrderSide.Sell;
                //sign = (x) => -x;
            }

            if (positionKeyInfo.isFractionable) // 以錢下單， 一元為最小單位，自動拆
            {
                //美元最小單位  小數兩位?
                var 下單錢 = Math.Round(diffMarketValue, 2, MidpointRounding.ToPositiveInfinity);  //多買 少賣

                orderQuantity = OrderQuantity.Notional(Math.Abs(下單錢));
            }
            else // 整數量
            {
                var 下單量 = Math.Ceiling(diffMarketValue / unitPrice);
                orderQuantity = OrderQuantity.FromInt64(Math.Abs((long)下單量));
            }


            try
            {
                await client.PostOrderAsync(side.Market(
                    symbol,
                    orderQuantity
                ));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            //var dict = new Dictionary<(OrderSide side, bool isFractionable), Action >()
            //{
            //    { 
            //        (OrderSide.Buy , true),
            //        ()=>{
            //            var 下單錢 = Math.Round(diffMarketValue, 2, MidpointRounding.ToPositiveInfinity);  //多買 少賣

            //            orderQuantityArg = 下單錢;
            //            if()

            //            orderQuantity = OrderQuantity.Notional(Math.Abs(下單錢));
            //        }
            //    },
            //    { 
            //        (OrderSide.Sell , true),
            //        ()=>{}
            //    },
            //    { 
            //        (OrderSide.Buy , true),
            //        ()=>{}
            //    },
            //    { 
            //        (OrderSide.Sell , true),
            //        ()=>{}
            //    },
            //};

        }
    }
}
