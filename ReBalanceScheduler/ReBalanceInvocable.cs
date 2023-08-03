using Coravel.Invocable;
using MyAlpacaStrategyLib;

namespace ReBalanceScheduler
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
        private readonly Logger _logger;
        public ReBalanceDayRepeatInvocable( ReBalanceOperation op, Logger logger)
        {
            _op = op;
            _logger = logger;
        }

        public async Task Invoke()
        {
            try
            {
                await _op.TryReBalanceAll();
            }
            catch (Exception ex)
            {
                //var msg = $"TryReBalanceAll EXCEPTION: \n{ex}";
                _logger.Log("TryReBalanceAll EXCEPTION:", ex.ToString());
            }
            //_logger.Log("========================");
        }
    }


    public class ReBalanceDayEndInvocable : IInvocable
    {
        public ReBalanceDayEndInvocable() { }
        public async Task Invoke()
        {
        }
    }
}
