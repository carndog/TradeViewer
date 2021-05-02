using NodaTime;

namespace TradesViewer
{
    public class ClosedTrade
    {
        public LocalDate DateOpened { get; set; }
        
        public LocalDate DateClosed { get; set; }
        
        public decimal Open { get; set; }
        
        public decimal Close { get; set; }
        
        public decimal Percentage { get; set; }
    }
}