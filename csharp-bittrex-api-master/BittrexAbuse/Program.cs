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
        public decimal Comission { get; set; }
        public decimal COIN { get; set; }
        public decimal USDTBalance { get; set; }
        public decimal OldBid { get; set; }
        public decimal OldAsk { get; set; }
        public decimal Purchase { get; set; }
        public decimal CurrentLast { get; set; }
        public GetMarketSummaryResponse MarketSummary { get; set; }
        public bool Fall { get; set; }
        public bool Rise { get; set; }
        public int FallCounter { get; set; }

        public void CheckAsk()
        {
            OldAsk = CurrentLast;
            CurrentLast = MarketSummary.Ask;
            //Console.WriteLine("Last = {0}", CurrentLast);
            //Console.WriteLine("Old = {0}", OldLast);
        }
        public void CheckBid()
        {
            OldBid = CurrentLast;
            CurrentLast = MarketSummary.Bid;
            //Console.WriteLine("Last = {0}", CurrentLast);
            //Console.WriteLine("Old = {0}", OldLast);
        }
        public bool Buy(Exchange e)
        {
            MarketSummary = e.GetMarketSummary("BTC");
            //var p = 0m;
            //if (MarketSummary.Low/MarketSummary.High >91 && MarketSummary.Low / MarketSummary.High < 93)
            //{
            //    p = 1.01m;
            //}
            CheckAsk();
            if (MarketSummary.Ask < MarketSummary.Low * 1.012m)
            {
                if (MarketSummary.Ask < OldAsk)
                {
                    Fall = true;
                    while (Fall)
                    {
                        if (MarketSummary.Ask <= OldAsk)
                        {
                            Thread.Sleep(1000);
                            Fall = true;
                            CheckAsk();
                        }
                        else
                        {
                            Fall = false;
                        }
                        MarketSummary = e.GetMarketSummary("BTC");
                    }
                }

                if (MarketSummary.Ask < MarketSummary.Low * 1.012m)
                {
                    InitialBalance = e.GetBalance("USDT").Available;
                    Comission = Decimal.Round((InitialBalance / MarketSummary.Ask) * MarketSummary.Ask * 0.0027m, 8);
                    var quantity = Decimal.Round(((InitialBalance - Comission) / MarketSummary.Ask), 8);
                    Console.WriteLine("Commission = {0}", Comission);
                    Console.WriteLine("USDTBalance = {0}", USDTBalance);
                    Console.WriteLine("Last.Ask = {0}", MarketSummary.Ask);
                    Console.WriteLine("InitialBalance = {0}", InitialBalance);

                    e.PlaceBuyOrder("BTC", quantity, MarketSummary.Ask);
                    Purchase = MarketSummary.Ask;
                    Console.WriteLine("buy quantity: {0} price: {1}", Decimal.Round(USDTBalance / MarketSummary.Ask, 8), MarketSummary.Ask);
                    COIN = quantity;
                    USDTBalance = e.GetBalance("USDT").Available;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool Sell(Exchange e)
        {
            MarketSummary = e.GetMarketSummary("BTC");
            CheckBid();
            //Console.WriteLine("Can recieve = {0}", (COIN * MarketSummary.Bid) - Comission);
            //Console.WriteLine("Coin =  {0}", COIN);
            //Console.WriteLine("Commission =  {0}", Comission);
            //Console.WriteLine("Last.bid =  {0}", MarketSummary.Bid);
            //Console.WriteLine("Profit =  {0}", ((COIN * MarketSummary.Bid) - Comission)-InitialBalance);

            if (MarketSummary.Bid > MarketSummary.High * 0.985m && ((COIN * MarketSummary.Bid) - Comission) > InitialBalance)
            {
                Rise = true;
                while (Rise)
                {

                    if (MarketSummary.Bid >= OldBid)
                    {
                        Thread.Sleep(1000);
                        CheckBid();
                        Rise = true;
                        Console.WriteLine("RAISING");
                    }
                    else
                    {
                        Rise = false;
                    }
                    MarketSummary = e.GetMarketSummary("NEO");
                }
            if (MarketSummary.Bid > MarketSummary.High*0.985m && ((COIN * MarketSummary.Bid) - Comission) > InitialBalance)
            {

                e.PlaceSellOrder("BTC", e.GetBalance("BTC").Available, MarketSummary.Bid);

                Console.WriteLine("sell quantity: {0} price: {1}", e.GetBalance("BTC").Available, MarketSummary.Bid);

                USDTBalance = e.GetBalance("USDT").Available;
                COIN = 0;

                Console.WriteLine("Profit =  {0}", ((COIN * MarketSummary.Bid) - Comission) - InitialBalance);

                Console.WriteLine();
                Console.WriteLine("InitialBalance = {0}", InitialBalance);
                Console.WriteLine("Profit =  {0}", ((COIN * MarketSummary.Bid) - Comission) - InitialBalance);
                Console.WriteLine("Comission = {0}", Comission);
                Console.WriteLine("Last.Bid = {0}", MarketSummary.Bid);

                return true;
                        
            }
            return false;
        }
            else
            {
                return false;
            }
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

            context.QuoteCurrency = "USDT";
            context.Simulate = false;

            e.Initialise(context);
            Score s = new Score();
            s.CurrentLast = 0;
            s.OldAsk = 0;
            s.OldBid = 0;
            s.USDTBalance = e.GetBalance("USDT").Available;
            s.COIN = e.GetBalance("BTC").Available;
            //Console.WriteLine("USDT balance = {0}",s.USDTBalance);
            //Console.WriteLine("BTC balance = {0}", s.COIN);
            s.InitialBalance = s.USDTBalance;
            s.MarketSummary = e.GetMarketSummary("BTC");
            s.Purchase = 0;
         

            //var c = Decimal.Round((7.94382931m / 15481.43808612m) * 15481.43808612m * 0.0028m, 8);
            //var quantity = Decimal.Round(((7.94382931m - c) / 15481.43808612m), 8);
            //Console.WriteLine(c);
            //Console.WriteLine(quantity);
            //var aas = (quantity * 15600.0m) - c;
            //Console.WriteLine(aas);

            var isBuy = false;
            var isSell = false;
            while (true)
            {
                while (!isBuy)
                {
                    isBuy = s.Buy(e);
                    isSell = false;
                    Thread.Sleep(3000);
                }
                while (!isSell)
                {
                    isSell = s.Sell(e);
                    isBuy = false;
                    Thread.Sleep(3000);
                }
            }
        }
    }
}