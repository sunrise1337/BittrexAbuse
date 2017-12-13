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
        public GetMarketSummaryResponse Last { get; set; }
        public bool Fall { get; set; }
        public bool Rise { get; set; }
        public int FallCounter { get; set; }
        public bool Buy(Exchange e)
        {
            Last = e.GetMarketSummary("BTC");
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
                    Last = e.GetMarketSummary("BTC");
                }
                if (Last.Ask < OldAsk && Last.Ask < (Last.High * 0.98m) || FallCounter > 0 && Last.Ask < (Last.High * 0.98m))
                {
                    InitialBalance = e.GetBalance("USDT").Available;
                    Comission = Decimal.Round((USDTBalance / Last.Ask) * Last.Ask * 0.0051m, 8);
                    var quantity = Decimal.Round(((USDTBalance - Comission) / Last.Ask) , 8);

                    e.PlaceBuyOrder("BTC", quantity, Last.Ask);
                    Purchase = Last.Ask;
                    Console.WriteLine("buy quantity: {0} price: {1}", Decimal.Round(USDTBalance / Last.Ask, 8), Last.Ask);
                    COIN = e.GetBalance("BTC").Available;
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
            Last = e.GetMarketSummary("BTC");
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
                    Last = e.GetMarketSummary("BTC");
                }

                if (Last.Bid > Purchase && COIN > 0 && ((COIN * Last.Bid) - Comission) > InitialBalance)
                {
                    //USDTBalance += COIN * Last.Ask;
                    //COIN = 0;
                    //Console.WriteLine("METHOD SELL");
                    //Console.WriteLine("Balance = {0} USDTBalance", USDTBalance);
                    // Purchase = 0;

                    e.PlaceSellOrder("BTC", e.GetBalance("BTC").Available, Last.Bid);

                    Console.WriteLine("sell quantity: {0} price: {1}", e.GetBalance("BTC").Available, Last.Bid);

                    USDTBalance = e.GetBalance("USDT").Available;
                    COIN = 0;
                    Profit = USDTBalance - InitialBalance;

                    Console.WriteLine("Profit: {0}", Profit);

                    return true;
                }
            }
            //else if (i==20)
            //{
            //    i = 0;
            //    USDTBalance += COIN * Last.Last;
            //    COIN = 0;
            //    Console.WriteLine("FORCE SELL");
            //    Console.WriteLine("Balance = {0} USDTBalance", USDTBalance);
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

            context.QuoteCurrency = "USDT";
            context.ApiKey = "8ef631a422f743eabcbefc288a9b20d0";
            context.Secret = "419b15331ef64491b76477522bd79770";
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
            s.Last = e.GetMarketSummary("BTC");
            s.Purchase = 0;
            var a = e.GetOrderHistory("BTC");

            //foreach (var item in a)
            //{
            //    Console.WriteLine("TimeStamp = {0}", item.TimeStamp);
            //    Console.WriteLine("Quantity = {0}", item.Quantity);
            //    Console.WriteLine("Price = {0}", item.Price);
            //    Console.WriteLine("Per Unit = {0}", item.PricePerUnit);
            //    Console.WriteLine("Limit = {0}", item.Limit);
            //    Console.WriteLine("Commission = {0}", item.Commission);
            //    Console.WriteLine("Exchange = {0}", item.Exchange);
            //    Console.WriteLine("Limit = {0}", item.Limit);
            //    Console.WriteLine("END OF ORDER");
            //    Console.WriteLine();
            //}
            //Console.WriteLine(a);


            //var c = Decimal.Round((8.04000000m / 15896.0m) * 15896.99m * 0.0050m, 8);
            //var quantity = Decimal.Round(((8.04000000m - c) / 15896.99m), 8);
            //Console.WriteLine(c);
            //Console.WriteLine(quantity);
            //var aas = (quantity * 15950.0m) - 0.01971770m;
            //Console.WriteLine(aas);
            //if (15950.0m > 15896.0m &&((quantity * 16190.0m) - c) > 8.04000000m)
            //{
            //    Console.WriteLine("PRodano");
            //}
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