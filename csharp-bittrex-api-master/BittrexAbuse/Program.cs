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
        public bool Buy(Exchange e)
        {
            MarketSummary = e.GetMarketSummary("NEO");

            if (MarketSummary.Ask < MarketSummary.Low * 1.03m)
            {

                InitialBalance = e.GetBalance("USDT").Available;
                Comission = Decimal.Round((InitialBalance / MarketSummary.Ask) * MarketSummary.Ask * 0.0029m, 8);
                var quantity = Decimal.Round(((InitialBalance - Comission) / MarketSummary.Ask), 8);
                Console.WriteLine("Commission = {0}", Comission);
                Console.WriteLine("USDTBalance = {0}", USDTBalance);
                Console.WriteLine("Last.Ask = {0}", MarketSummary.Ask);
                Console.WriteLine("InitialBalance = {0}", InitialBalance);

                e.PlaceBuyOrder("NEO", quantity, MarketSummary.Ask);
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
        public bool Sell(Exchange e)
        {
            MarketSummary = e.GetMarketSummary("NEO");

            //Console.WriteLine("Can recieve = {0}", (COIN * MarketSummary.Bid) - Comission);
            //Console.WriteLine("Coin =  {0}", COIN);
            //Console.WriteLine("Commission =  {0}", Comission);
            //Console.WriteLine("Last.bid =  {0}", MarketSummary.Bid);
            //Console.WriteLine("Profit =  {0}", ((COIN * MarketSummary.Bid) - Comission)-InitialBalance);
            if (MarketSummary.Bid > MarketSummary.High*0.97m && ((COIN * MarketSummary.Bid) - Comission) > InitialBalance)
            {

                e.PlaceSellOrder("NEO", e.GetBalance("NEO").Available, MarketSummary.Bid);

                Console.WriteLine("sell quantity: {0} price: {1}", e.GetBalance("NEO").Available, MarketSummary.Bid);

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
            s.COIN = e.GetBalance("NEO").Available;
            //Console.WriteLine("USDT balance = {0}",s.USDTBalance);
            //Console.WriteLine("BTC balance = {0}", s.COIN);
            s.InitialBalance = s.USDTBalance;
            s.MarketSummary = e.GetMarketSummary("NEO");
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