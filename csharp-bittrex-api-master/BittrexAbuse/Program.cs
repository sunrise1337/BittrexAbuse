using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bittrex;

namespace BittrexAbuse
{
    class Program
    {
        static Exchange ex = new Exchange();
        static ExchangeContext context = new ExchangeContext();

        static void Main(string[] args)
        {
            context.ApiKey = "8ef631a422f743eabcbefc288a9b20d0";
            context.Secret = "419b15331ef64491b76477522bd79770";
            context.Simulate = false;
            context.QuoteCurrency = "BTC";

            ex.Initialise(context);

            var tmp = ex.GetMarketSummary("ZEC");

            var c = (tmp.High + tmp.Low) / 2;

            Console.WriteLine(c);
        }


    }
}
