namespace TradesViewer
{
    public class AssetSummary
    {
        public string Asset { get; set; }
        
        public int NumberOfTrades { get; set; }
        
        public int NumberOfTradesOverTenPercent { get; set; }
        
        public decimal AveragePercentGainOverTenPercent { get; set; }
        
        public int NumberOfWins { get; set; }
        
        public decimal AverageWinPercentage { get; set; }
        
        public int NumberOfLosses { get; set; }
        
        public decimal AverageLosingPercent { get; set; }
        
        public int[] NumberInMonth { get; set; }
    }
}