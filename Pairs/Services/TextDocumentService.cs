using Pairs.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pairs.Services
{
    public class TextDocumentService
    {
        public void GenerateReport(List<PairHistoryReport> pairHistoryReports)
        {
            string fileTimeStamp = (DateTime.Now.ToString("MMMM dd") + " " + DateTime.Now.ToString("h:mm tt")).Replace(" ", "-").Replace(":", "-");
            string fileName = $"C:\\Users\\corberso\\source\\repos\\Pairs\\Pairs\\Reports\\PairReport{fileTimeStamp}.txt";

            foreach (PairHistoryReport pairHistoryReport in pairHistoryReports)
            {
                string percentPaired = ((100 * pairHistoryReport.PairDays.Count) / pairHistoryReport.WorkDaysExcludingInnonvation.Count).ToString();
                string percentSolo = ((100 * pairHistoryReport.SoloDays.Count) / pairHistoryReport.WorkDaysExcludingInnonvation.Count).ToString();
                string contractorPaired = ((100 * pairHistoryReport.PairWithContractorsDays.Count) / pairHistoryReport.PairDays.Count).ToString();
                string vizientPaired = ((100 * pairHistoryReport.PairWithVizientDays.Count) / pairHistoryReport.PairDays.Count).ToString();
                string moreThanOnePaired = ((100 * pairHistoryReport.MoreThanOnePairedWithDays.Count) / pairHistoryReport.PairDays.Count).ToString();
                string innovation = ((100 * pairHistoryReport.InnovationDays.Count) / pairHistoryReport.WorkDays.Count).ToString();
                string workDaysExcludingInnovation = ((100 * pairHistoryReport.WorkDaysExcludingInnonvation.Count) / pairHistoryReport.WorkDays.Count).ToString();

                using (StreamWriter wr = File.AppendText(fileName))
                {
                    wr.WriteLine($"{pairHistoryReport.Name} Report");
                    wr.WriteLine($"Worked Days: {pairHistoryReport.WorkDays.Count}");
                    wr.WriteLine($"Paired Days: {percentPaired}%, {pairHistoryReport.WorkDays.Count}");
                    wr.WriteLine($"Solo Days: {percentSolo}%, {pairHistoryReport.SoloDays.Count}");
                    wr.WriteLine($"Paired With Contractor Worker Days: {contractorPaired}%, {pairHistoryReport.PairWithContractorsDays.Count}");
                    wr.WriteLine($"Paired With Vizient Worker Days: {vizientPaired}%, {pairHistoryReport.PairWithVizientDays.Count}");
                    wr.WriteLine($"Paired With More Than One Worker Days: {moreThanOnePaired}%, {pairHistoryReport.MoreThanOnePairedWithDays.Count}");
                    wr.WriteLine($"Innovation Days: {innovation}%, {pairHistoryReport.InnovationDays.Count}");
                    wr.WriteLine($"Worked Days Excluding Innovation: {workDaysExcludingInnovation}%, {pairHistoryReport.WorkDaysExcludingInnonvation.Count}");
                    foreach(var pair in pairHistoryReport.PairOccurences)
                    {
                        string pairedWith = ((100 * pair.Value) / pairHistoryReport.WorkDaysExcludingInnonvation.Count).ToString();
                        wr.WriteLine($"Worked Days With {pair.Key}: {pairedWith}%, {pair.Value}");
                    }
                    wr.WriteLine();
                }
            }
        }
    }
}
