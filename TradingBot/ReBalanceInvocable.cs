using Coravel.Invocable;
using MyAlpacaStrategyLib;

namespace TradingBot
{
    public class ReBalanceInvocable : IInvocable
    {
        public async Task Invoke()
        {
            var a = new ReBalanceOperation();
            await a.TryReBalanceAll();
            Console.WriteLine("========================");
        }
    }
}
