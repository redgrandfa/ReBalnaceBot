using Coravel.Invocable;
using MyAlpacaStrategyLib;

namespace TradingBot
{
    public class ReBalanceDayStartInvocable : IInvocable
    {
        public async Task Invoke()
        {
            ReBalanceOperation.ResetTodayPositionSides();
            await ReBalanceOperation.ExecOnce();
        }
    }
    public class ReBalanceDayRepeatInvocable : IInvocable
    {
        public async Task Invoke()
        {
            await ReBalanceOperation.ExecOnce();
        }
    }
}
