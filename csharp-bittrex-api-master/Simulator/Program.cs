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
        public bool Fall { get; set; }
        public bool Rise { get; set; }
        public int FallCounter { get; set; }
        public void Buy(Exchange e)
        {

            Last = e.GetMarketSummary("BTC");
            Check();
            if (Last.Last < OldLast)
            {
                Fall = true;
                FallCounter = 0;
                while (Fall)
                {                                
                    if (Last.Last <= OldLast)
                    {
                        Thread.Sleep(1000);
                        Fall = true;
                        Console.WriteLine("FALLING");
                        Check();
                        FallCounter++;                     
                    }
                    else
                    {
                        Fall = false;
                    }
                    Last = e.GetMarketSummary("BTC");
                }
                if (Last.Last < OldLast && Last.Last < (Last.High * 0.975m) || FallCounter>0 && Last.Last < (Last.High * 0.975m))
                {

                    Purchase = Last.Last;
                    BTC = USDT / Last.Last;
                    USDT = USDT - (BTC * Last.Last);
                    Console.WriteLine("METHOD BUY");
                    Console.WriteLine("Balance = {0} BTC", BTC);
                    Console.WriteLine("Balance = {0} USDT", USDT);
                }
            }
        }
        public bool Sell(Exchange e)
        {
            Last = e.GetMarketSummary("BTC");
            Check();
            Console.WriteLine("Purchase: {0}",Purchase);
            if (Last.Last > Purchase)
            {
                Rise = true;
                while (Rise)
                {
                   
                  
                    if (Last.Last >= OldLast)
                    {
                        Thread.Sleep(1000);
                        Check();
                        Rise = true;
                        Console.WriteLine("RAISING");
                        

                    }
                    else
                    {
                        Rise = false;
                    }
                    Last = e.GetMarketSummary("BTC");
                }

                if (Last.Last > Purchase && BTC > 0)
                {
                    USDT += BTC * Last.Last;
                    BTC = 0;
                    Console.WriteLine("METHOD SELL");
                    Console.WriteLine("Balance = {0} USDT", USDT);
                    Purchase = 0;

                    return true;
                }
            }
            //else if (i==20)
            //{
            //    i = 0;
            //    USDT += BTC * Last.Last;
            //    BTC = 0;
            //    Console.WriteLine("FORCE SELL");
            //    Console.WriteLine("Balance = {0} USDT", USDT);
            //}
            //i++;
            return false;
        }
        public void Check()
        {
            OldLast = CurrentLast;
            CurrentLast = Last.Last;
            Console.WriteLine("Last = {0}", CurrentLast);
            Console.WriteLine("Old = {0}", OldLast);
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
            int i = 0;
            s.Last = e.GetMarketSummary("BTC");
            while (s.USDT < 51000)
            {
                if (s.USDT > 1)
                {
                    s.Buy(e);

                }

                //Console.WriteLine("Purchase = {0}", s.Purchase);
              
                Thread.Sleep(4000);
                if (s.Purchase > 0)
                {
                    s.Sell(e);
                }

            }

            Console.WriteLine("USDT = {0}", s.USDT);


        }
    }
}
