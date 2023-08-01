using Coravel.Invocable;
using MyAlpacaStrategyLib;

namespace TradingBot
{
    public class ReBalanceDayStartInvocable : IInvocable
    {
        public ReBalanceDayStartInvocable(){ }
        public async Task Invoke()
        {
            ReBalanceOperation.ResetTodayPositionSides();
        }
    }
    public class ReBalanceDayRepeatInvocable : IInvocable
    {
        private readonly ReBalanceOperation _op;
        public ReBalanceDayRepeatInvocable( ReBalanceOperation op)
        {
            _op = op;
        }

        public async Task Invoke()
        {
            await _op.ExecOnce();
        }
    }
}
