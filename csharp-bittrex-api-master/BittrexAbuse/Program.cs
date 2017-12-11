using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bittrex;
using System.Diagnostics;
using System.Threading;
using System.Globalization;

namespace BittrexAbuse
{
    public class Score
    {
        public decimal InitialBalance { get; set; }
        public decimal Profit { get; set; }
        public decimal COIN { get; set; }
        public decimal BTCBalance { get; set; }
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
            CheckAsk();

            // Console.WriteLine("Last ask: {0}", Last.Ask);
            // Console.WriteLine("Old ask: {0}", OldAsk);

            if (Last.Ask < OldAsk)
            {
                Fall = true;
                FallCounter = 0;
                while (Fall)
                {
                    if (Last.Ask <= OldAsk)
                    {
                        Thread.Sleep(1000);
                        Fall = true;

                        CheckAsk();
                        FallCounter++;
                    }
                    else
                    {
                        Fall = false;
                    }
                    Last = e.GetMarketSummary("NEO");
                }
                if (Last.Ask < OldAsk && Last.Ask < (Last.High * 0.98m) || FallCounter > 0 && Last.Ask < (Last.High * 0.98m))
                {
                    var a = Decimal.Round((BTCBalance - 0.00000130m) / Last.Ask, 8);

                    e.PlaceBuyOrder("NEO", a, Last.Ask);

                    Console.WriteLine("buy quantity: {0} price: {1}", Decimal.Round(BTCBalance / Last.Ask, 8), Last.Ask);
                    COIN = e.GetBalance("NEO").Available;
                    BTCBalance = e.GetBalance("BTC").Available;
                    Purchase = Last.Ask;
                }
            }
        }
        public bool Sell(Exchange e)
        {
            Last = e.GetMarketSummary("NEO");
            CheckBid();

            // Console.WriteLine("Last bid: {0}", Last.Bid);

            if (Last.Bid > Purchase)
            {
                Rise = true;
                while (Rise)
                {


                    if (Last.Bid >= OldBid)
                    {
                        Thread.Sleep(1000);
                        CheckBid();
                        Rise = true;
                        //Console.WriteLine("RAISING");


                    }
                    else
                    {
                        Rise = false;
                    }
                    Last = e.GetMarketSummary("NEO");
                }

                if (Last.Bid > Purchase && COIN > 0)
                {
                    //BTCBalance += COIN * Last.Ask;
                    //COIN = 0;
                    //Console.WriteLine("METHOD SELL");
                    //Console.WriteLine("Balance = {0} BTCBalance", BTCBalance);
                    // Purchase = 0;

                    e.PlaceSellOrder("NEO", e.GetBalance("NEO").Available, Last.Bid);

                    Console.WriteLine("sell quantity: {0} price: {1}", e.GetBalance("NEO").Available, Last.Bid);

                    BTCBalance = e.GetBalance("BTC").Available;

                    Profit = BTCBalance - InitialBalance;

                    Console.WriteLine("Profit: {0}", Profit);

                    return true;
                }
            }
            //else if (i==20)
            //{
            //    i = 0;
            //    BTCBalance += COIN * Last.Last;
            //    COIN = 0;
            //    Console.WriteLine("FORCE SELL");
            //    Console.WriteLine("Balance = {0} BTCBalance", BTCBalance);
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
            CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            Thread.CurrentThread.CurrentCulture = customCulture;
            ExchangeContext context = new ExchangeContext();

            context.QuoteCurrency = "BTC";
            context.ApiKey = "8ef631a422f743eabcbefc288a9b20d0";
            context.Secret = "419b15331ef64491b76477522bd79770";
            context.Simulate = false;

            e.Initialise(context);
            Score s = new Score();
            s.CurrentLast = 0;
            s.OldAsk = 0;
            s.OldBid = 0;
            s.BTCBalance = e.GetBalance("BTC").Available;
            s.COIN = e.GetBalance("NEO").Available;
            s.InitialBalance = s.BTCBalance;

            s.Last = e.GetMarketSummary("NEO");

            while (true)
            {
                if (s.BTCBalance > 0.00025000m)
                {
                    s.Buy(e);
                }

                if (s.Purchase > 0 && e.GetBalance("NEO").Available > 0)
                {
                    s.Sell(e);
                }

                Thread.Sleep(3000);
            }
        }
    }
}