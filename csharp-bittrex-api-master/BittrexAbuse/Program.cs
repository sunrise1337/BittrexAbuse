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
        static void Main(string[] args)
        {

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
