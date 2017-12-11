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
        public decimal OldBid { get; set; }
        public decimal OldAsk { get; set; }
        public decimal Purchase { get; set; }
        public decimal CurrentLast { get; set; }
        public GetMarketSummaryResponse Last { get; set; }
        public bool Fall { get; set; }
        public bool Rise { get; set; }
        public int FallCounter { get; set; }
        public void Buy(Exchange e)
        {

            Last = e.GetMarketSummary("NEO");
            CheckBid();
            if (Last.Bid < OldBid)
            {
                Fall = true;
                FallCounter = 0;
                while (Fall)
                {
                    if (Last.Bid <= OldBid)
                    {
                        Thread.Sleep(1000);
                        Fall = true;

                        CheckBid();
                        FallCounter++;
                    }
                    else
                    {
                        Fall = false;
                    }
                    Last = e.GetMarketSummary("NEO");
                }
                if (Last.Bid < OldBid && Last.Bid < (Last.High * 0.98m) || FallCounter > 0 && Last.Bid < (Last.High * 0.98m))
                {

                    Purchase = Last.Bid;
                    BTC = USDT / Last.Bid;
                    USDT = USDT - (BTC * Last.Bid);
                    Console.WriteLine("METHOD BUY");
                    Console.WriteLine("Balance = {0} ZEC", BTC);
                    Console.WriteLine("Balance = {0} USDT", USDT);
                }
            }
        }
        public bool Sell(Exchange e)
        {
            Last = e.GetMarketSummary("NEO");
            CheckAsk();
            //Console.WriteLine("Purchase: {0}", Purchase);
            if (Last.Ask > Purchase)
            {
                Rise = true;
                while (Rise)
                {


                    if (Last.Ask >= OldAsk)
                    {
                        Thread.Sleep(1000);
                        CheckAsk();
                        Rise = true;
                        //Console.WriteLine("RAISING");


                    }
                    else
                    {
                        Rise = false;
                    }
                    Last = e.GetMarketSummary("NEO");
                }

                if (Last.Ask > Purchase && BTC > 0)
                {
                    USDT += BTC * Last.Ask;
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
        public void CheckAsk()
        {
            OldAsk = CurrentLast;
            CurrentLast = Last.Ask;
            //Console.WriteLine("Last = {0}", CurrentLast);
            //Console.WriteLine("Old = {0}", OldLast);
        }
        public void CheckBid()
        {
            OldBid = CurrentLast;
            CurrentLast = Last.Bid;
            //Console.WriteLine("Last = {0}", CurrentLast);
            //Console.WriteLine("Old = {0}", OldLast);
        }

    }

    class Program
    {
        static Exchange e = new Exchange();
        static void Main(string[] args)
        {

            //ExchangeContext context = new ExchangeContext();
            //context.QuoteCurrency = "USDT";
            //e.Initialise(context);
            //Score s = new Score();
            //s.CurrentLast = 0;
            //s.OldAsk = 0;
            //s.OldBid = 0;
            //s.USDT = 500;
            //s.BTC = 0;
            //int i = 0;
            //s.Last = e.GetMarketSummary("NEO");
            //while (s.USDT < 51000)
            //{
            //    if (s.USDT > 1)
            //    {
            //        s.Buy(e);

            //    }
            //    Thread.Sleep(4000);
            //    if (s.Purchase > 0)
            //    {
            //        s.Sell(e);
            //    }
            //}
            //decimal a = 0.0001m;
            //Console.WriteLine("USDT = {0}", s.USDT);
            var USDTBalance = Math.Floor(9.111119999m);
            Console.WriteLine(USDTBalance);
        }
    }
}
