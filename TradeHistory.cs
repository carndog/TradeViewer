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

            summary.Asset = Asset;
            summary.NumberOfTrades = _closedTrades.Count;
            
            IEnumerable<ClosedTrade> winningTrades = _closedTrades.Where(x => x.Percentage > 0).ToList();
            IEnumerable<ClosedTrade> losingTrades = _closedTrades.Where(x => x.Percentage < 0).ToList();
            IEnumerable<ClosedTrade> winsOverTenPercent = _closedTrades.Where(x => x.Percentage >= 10).ToList();
            
            summary.AverageWinPercentage = !winningTrades.Any() ? 0 : winningTrades.Average(x => x.Percentage);
            summary.AverageLosingPercent = !losingTrades.Any() ? 0 : losingTrades.Average(x => x.Percentage);
            summary.NumberOfLosses = losingTrades.Count();
            summary.NumberOfWins = winningTrades.Count();
            summary.AveragePercentGainOverTenPercent = !winsOverTenPercent.Any() ? 0 : winsOverTenPercent.Average(x => x.Percentage);
            summary.NumberOfTradesOverTenPercent = winsOverTenPercent.Count();

            summary.AverageWinPercentage = Math.Round(summary.AverageWinPercentage, 2);
            summary.AverageLosingPercent = Math.Round(summary.AverageLosingPercent, 2);
            summary.AveragePercentGainOverTenPercent = Math.Round(summary.AveragePercentGainOverTenPercent, 2);

            summary.NumberInMonth = new int[12];

            TimeSpan oneYear = new TimeSpan(365, 0, 0, 0);
            MonthIterator months = new MonthIterator(DateTime.Now.Subtract(oneYear));

            int i = 0;
            foreach (DateTime dateTime in months)
            {
                int count = _closedTrades.Count(x =>
                    x.DateOpened.Month == dateTime.Month && x.DateOpened.Year == dateTime.Year);
                summary.NumberInMonth[i] = count;
                i++;
            }

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

                        if (_closedTrades.All(x => x.DateOpened != closedTrade.DateOpened))
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