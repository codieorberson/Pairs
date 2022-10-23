namespace Pairs.Models
{
    public class PairHistoryReport
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public List<DateTime> SoloDays { get; set; }
        public List<DateTime> PairDays { get; set; }
        public List<DateTime> WorkDays { get; set; }
        public List<DateTime> InnovationDays { get; set; }
        public List<DateTime> WorkDaysExcludingInnonvation { get; set; }
        public List<DateTime> PairWithContractorsDays { get; set; }
        public List<DateTime> PairWithVizientDays { get; set; }
        public List<DateTime> MoreThanOnePairedWithDays { get; set; }
        public Dictionary<string, int> PairOccurences { get; set; }

        
    }
}
