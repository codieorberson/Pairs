using Pairs.Models;
using Pairs.Services;


//Console.WriteLine("Please Enter the name of your CSV File");
//string csvFileName = Console.ReadLine();
string csvFileName = "C:\\Users\\corberso\\source\\repos\\Pairs\\Pairs\\SSOPairs2022.xlsx";
Console.WriteLine("Please enter the type of report requested");
Console.WriteLine("Full Report? (Enter 0)");
Console.WriteLine("Report based on a time range? (Enter 1)");
//Console.WriteLine("Report based on number of last work days? (Enter 2)");
int reportOption = Int32.Parse(Console.ReadLine());

DateTime? startDate = DateTime.Now;
DateTime? endDate = DateTime.Now;
int? numLastWorkDays = null;

if (reportOption == 1)
{
    Console.WriteLine("Please Enter the Start Date (MM/DD)");
    string startDateInput = Console.ReadLine();
    startDate = DateTime.Parse($"{startDateInput}/{DateTime.Now.Year}");

    Console.WriteLine("Please Enter the End Date (MM/DD)");
    string endDateInput = Console.ReadLine();
    endDate = DateTime.Parse($"{endDateInput}/{DateTime.Now.Year}");

}
DateTime now = DateTime.Now;

//if (reportOption == 2)
//{
//    Console.WriteLine("Please Enter the amount of last work days");
//    numLastWorkDays = Int32.Parse(Console.ReadLine());
//}
TextDocumentService textDocumentService = new TextDocumentService();

ExcelService excelService = new ExcelService(csvFileName);

//Construct List of Team Members
List<TeamMember> teamMembers = excelService.ConstructTeamMembers();

//Contruct Pair History
List<PairHistory> pairsHistory = excelService.ConstructPairHistory();

//Filter Pair History based on time range option chosen
if(reportOption == 1) pairsHistory = pairsHistory.Where(x=>x.Date > startDate).Where(x=>x.Date<endDate).ToList();
if (reportOption == 2)
{
    int startWorkDays = (int)(pairsHistory.Select(x => x.Day).Max() - numLastWorkDays);
    pairsHistory = pairsHistory.Where(x => x.Day > startWorkDays).ToList();
}


List<string> contactorIds = teamMembers.Where(x => x.Contractor).Select(x => x.Id).ToList();
List<string> vizientIds = teamMembers.Where(x => x.Contractor==false).Select(x => x.Id).ToList();

List<PairHistoryReport> pairHistoryReports = new List<PairHistoryReport>();


foreach(TeamMember teamMember in teamMembers)
{
    pairHistoryReports.Add(new PairHistoryReport()
    {
        Name = teamMember.Name,
        Id = teamMember.Id,
        SoloDays = new List<DateTime>(),
        PairDays = new List<DateTime>(),
        WorkDays = new List<DateTime>(),
        InnovationDays = new List<DateTime>(),
        WorkDaysExcludingInnonvation = new List<DateTime>(),
        PairWithContractorsDays = new List<DateTime>(),
        PairWithVizientDays = new List<DateTime>(),
        MoreThanOnePairedWithDays = new List<DateTime>(),
        PairOccurences = new Dictionary<string, int>()
    });

}

foreach(PairHistory pairHistory in pairsHistory)
{
    foreach(PairHistoryReport pairHistoryReport in pairHistoryReports)
    {
        bool dayInnovated = false;
        bool daySolo = false;
        bool dayPaired = false;
        bool contractorPaired = false;
        bool vizientPaired = false;
        bool moreThanOnePairedWith = false;

        //Add Innovation Days
        if (pairHistory.InnovationMembers.Contains(pairHistoryReport.Id))
        {
            dayInnovated = true;
            
        }
            
        foreach(string pair in pairHistory.Pairs)
        {   
            //Set Flags for Solo Day
            if (pair.Length == 1 && pair == pairHistoryReport.Id)
            {
                daySolo = true;
            }

            //Add PairOccurences and Set Flags for Paired, PairedWithContractor, PairedWithVizient Days
            if (pair.Length > 1 && pair.Contains(pairHistoryReport.Id))
            {
                dayPaired = true;

                //Construct List of Team Member Ids Paired With
                List<string> membersPairedWith = new List<string>();
                string membersPairedWithString = pair.Replace(pairHistoryReport.Id, "");
                foreach (char pairmemberId in membersPairedWithString)
                {
                    membersPairedWith.Add(pairmemberId.ToString());
                }

                //Set Flag for MoreThanOnePairedWith
                if (membersPairedWith.Count > 1) moreThanOnePairedWith = true;

                //Set Flag for PairedWithContractors
                if (membersPairedWith.Any(x => contactorIds.Any(y => y == x))) contractorPaired = true;

                //Set Flag for PairedWithVizient
                if (membersPairedWith.Any(x => vizientIds.Any(y => y == x))) vizientPaired = true;

                //Add Paired Occurences
                foreach (string memberPairedWith in membersPairedWith)
                {
                    string memberNamePairedWith = teamMembers.Where(x => x.Id == memberPairedWith).Select(x => x.Name).ToList()[0].ToString();
                    if (pairHistoryReport.PairOccurences.ContainsKey(memberNamePairedWith)) pairHistoryReport.PairOccurences[memberNamePairedWith] += 1;
                    else pairHistoryReport.PairOccurences.Add(memberNamePairedWith, 1);
                }
            }
        }

        //Add Work Day
        if (dayInnovated || daySolo || dayPaired) pairHistoryReport.WorkDays.Add(pairHistory.Date);
        //Add Innovation Day
        if (dayInnovated) pairHistoryReport.InnovationDays.Add(pairHistory.Date);
        //Add NotInnovation Day
        if ((daySolo || dayPaired) && !dayInnovated) pairHistoryReport.WorkDaysExcludingInnonvation.Add(pairHistory.Date);
        //Add Solo Day
        if (daySolo) pairHistoryReport.SoloDays.Add(pairHistory.Date);
        //Add Pair Day
        if(dayPaired) pairHistoryReport.PairDays.Add(pairHistory.Date);
        //Add ContractorPaired Day
        if(contractorPaired) pairHistoryReport.PairWithContractorsDays.Add(pairHistory.Date);
        //Add VizientPaired Day
        if(vizientPaired) pairHistoryReport.PairWithVizientDays.Add(pairHistory.Date);
        //Add MoreThanOnePairedWith Day
        if (moreThanOnePairedWith) pairHistoryReport.MoreThanOnePairedWithDays.Add(pairHistory.Date);

    }
}

//Remove Reports for Members not active in Time Range Selected
pairHistoryReports = pairHistoryReports.Where(x => x.WorkDays.Count > 0).ToList();
textDocumentService.GenerateReport(pairHistoryReports);

Console.WriteLine($"Run Time: {DateTime.Now - now}");
