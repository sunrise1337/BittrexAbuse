using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bittrex;
using System.Diagnostics;
using System.Threading;

namespace BittrexAbuse
{
    public class Score
    {
        public decimal BTC { get; set; }
        public decimal USDT { get; set; }
        public decimal OldLast { get; set; }
        public decimal Purchase { get; set; }
        public decimal CurrentLast { get; set; }
        public GetMarketSummaryResponse Last { get; set; }
        public void Buy(Exchange e)
        {

            Last = e.GetMarketSummary("BTC");
            Check();

            if (Last.Last < OldLast && Last.Last < (Last.High * 098m))
            {

                Purchase = Last.Last;
                BTC = USDT / Last.Last;
                USDT = USDT - (BTC * Last.Last);
                Console.WriteLine("METHOD BUY");
                Console.WriteLine("Balance = {0} BTC", BTC);
                Console.WriteLine("Balance = {0} USDT", USDT);

                
            }

        }
        public bool Sell(Exchange e)
        {
            Last = e.GetMarketSummary("BTC");
            if (Last.Last > Purchase && BTC > 0)
            {
                USDT += BTC * Last.Last;
                BTC = 0;
                Console.WriteLine("METHOD SELL");
                Console.WriteLine("Balance = {0} USDT", USDT);

                return true;
            }

            return false;
        }
        public void Check()
        {
            OldLast = CurrentLast;
            CurrentLast = Last.Last;
        }

    }

    class Program
    {
        static Exchange e = new Exchange();
        static void Main(string[] args)
        {

            ExchangeContext context = new ExchangeContext();
            context.QuoteCurrency = "USDT";
            e.Initialise(context);
            Score s = new Score();
            s.CurrentLast = 0;
            s.OldLast = 0;
            s.USDT = 500;
            s.BTC = 0;
       
            s.Last = e.GetMarketSummary("BTC");
            while (s.USDT < 51000)
            {
                if (s.USDT > 1)
                {
                    s.Buy(e);

                }

                Console.WriteLine("Purchase = {0}", s.Purchase);
                Console.WriteLine("Last = {0}", s.Last.Last);
                Console.WriteLine("Old = {0}", s.OldLast);
                Thread.Sleep(5000);
                s.Sell(e);
            }

            Console.WriteLine("USDT = {0}", s.USDT);


        }
    }
}
