using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bittrex;
using System.Diagnostics;

namespace BittrexAbuse
{
    class Program
    {
        static Exchange e = new Exchange();
        const string apiKey = "8ef631a422f743eabcbefc288a9b20d0";
        const string apiSecret = "419b15331ef64491b76477522bd79770";
        static decimal? purchase;
        static decimal oldLast;
        static decimal currentLast;
        static void Main(string[] args)
        {

            ExchangeContext context = new ExchangeContext();
            context.ApiKey = apiKey;
            context.Secret = apiSecret;
            context.QuoteCurrency = "USDT";      
            e.Initialise(context);


            //Exchange ex = new Exchange();
            //ExchangeContext context2 = new ExchangeContext();
            //context.ApiKey = "8ef631a422f743eabcbefc288a9b20d0";
            //context.Secret = "419b15331ef64491b76477522bd79770";
            //context2.Simulate = true;
            //context2.QuoteCurrency = "USDT";

            //ex.Initialise(context2);

            //var tmp = ex.GetMarketSummary("ZEC");

            //var mid = (tmp.High + tmp.Low) / 2;
            //var last = tmp.Last;
            //Console.WriteLine(last);
        }
        static void CheckLast()
        {
            oldLast = currentLast;
            currentLast = e.GetMarketSummary("ZEC").Last;
        }
        static void BuyOrder()
        {
            var tmp = e.GetMarketSummary("ZEC");
            if (tmp.Last<(tmp.High * 0.97m) && tmp.Last<oldLast)
            {
                e.PlaceBuyOrder("ZEC", 0.000000001m, tmp.Last);
                purchase = tmp.Last;
            }
        }
    }
}
