using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlpacaStrategyLib
{
    public class Logger
    {
        public readonly IMemoryCacheRepository _cacheRepo;
        public Logger(IMemoryCacheRepository cacheRepo)
        {
            _cacheRepo = cacheRepo;
        }

        public void Log(string appendKey , string msg)
        {
            Console.WriteLine(msg);

            var key = $"{DateTime.UtcNow.AddHours(8).ToString("yy/MM/dd-hh:mm:ss.ffff")}{appendKey}";
            _cacheRepo.Set(key, msg);
        }
        public void Log(string msg)
        {
            Log("", msg);
        }
    }
}
