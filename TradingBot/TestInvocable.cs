using Coravel.Invocable;

namespace TradingBot
{
    public class TestInvocable : IInvocable
    {
        //有支援DI，需要的就注進來
        //private readonly XXContext _context;
        //public CreateInvoiceTask(XXContext context)
        //{
        //    _context = context;
        //}

        //public Task Invoke()
        //{
        //    return Task.CompletedTask;
        //}
        public async Task Invoke()

        {
            Console.WriteLine("類別裡面 同步");
        }
    }
}
