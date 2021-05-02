using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;
using NodaTime.Text;

namespace TradesViewer
{
    public class TradeHistory
    {
        private readonly List<ClosedTrade> _closedTrades = new List<ClosedTrade>(10);

        public TradeHistory(string asset)
        {
            Asset = asset;
        }

        public AssetSummary GetSummary()
        {
            AssetSummary summary = new AssetSummary();

            return summary;
        }

        public string Asset { get; }

        public void ParseTrade(string line)
        {
            try
            {
                if (line.StartsWith("Close"))
                {
                    string[] items = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    if (items.Length == 6)
                    {
                        LocalDate openDate = LocalDatePattern.Iso.Parse(items[1]).Value;
                        LocalDate closeDate = LocalDatePattern.Iso.Parse(items[2]).Value;
                        decimal open = decimal.Parse(items[3]);
                        decimal close = decimal.Parse(items[4]);
                        decimal change = decimal.Parse(items[5]);

                        ClosedTrade closedTrade = new ClosedTrade()
                        {
                            DateOpened = openDate,
                            DateClosed = closeDate,
                            Open = open,
                            Close = close,
                            Percentage = change
                        };

                        if (_closedTrades.All(x => x.Open != open))
                        {
                            _closedTrades.Add(closedTrade);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}