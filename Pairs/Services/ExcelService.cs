using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using Pairs.Models;


namespace Pairs.Services
{
    public class ExcelService
    {
        public ExcelWorksheet teamMemberSheet { get; set; }
        public ExcelWorksheet pairHistorySheet { get; set; }
        public ExcelWorksheets workSheets { get; set; }


        public ExcelService(string fileLocation)
        {
            ConstructWorkSheets(fileLocation);
        }

        public List<TeamMember> ConstructTeamMembers()
        {
            List<TeamMember> teamMembers = new List<TeamMember>();
            int columns = teamMemberSheet.Dimension.End.Column;
            int rows = teamMemberSheet.Dimension.End.Row;
            for (int rowIterator = 2; rowIterator <= rows; rowIterator++)
            {
                var teamMember = new TeamMember
                {
                    Name = teamMemberSheet.Cells[rowIterator, 1].Value?.ToString(),
                    Id = teamMemberSheet.Cells[rowIterator, 2].Value.ToString(),
                    Contractor = Convert.ToBoolean(teamMemberSheet.Cells[rowIterator, 3].Value)
                };
                teamMembers.Add(teamMember);
            }
            return teamMembers;
        }

        public List<PairHistory> ConstructPairHistory()
        {
            List<PairHistory> pairsHistory = new List<PairHistory>();
            int columns = pairHistorySheet.Dimension.End.Column;
            int rows = pairHistorySheet.Dimension.End.Row;
            for (int rowIterator = 2; rowIterator <= rows; rowIterator++)
            {
                var pairhistory = new PairHistory
                {
                    Day = Convert.ToInt32(pairHistorySheet.Cells[rowIterator, 1].Value),
                    Pairs = ConvertStringsToListString(pairHistorySheet.Cells[rowIterator, 2].Value.ToString()),
                    InnovationMembers = ConvertStringToListString(pairHistorySheet.Cells[rowIterator, 3].Value),
                    Date = Convert.ToDateTime(pairHistorySheet.Cells[rowIterator, 4].Value),



                };
                pairsHistory.Add(pairhistory);
            }
            return pairsHistory;
        }
        

        private void ConstructWorkSheets(string csvFileName)
        {
            var package = new ExcelPackage(csvFileName);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            workSheets = package.Workbook.Worksheets;
            teamMemberSheet = workSheets[0];
            pairHistorySheet = workSheets[1];
        }

        private List<string> ConvertStringsToListString(string value)
        {
            return new List<string>(value.Split(' '));
        }
        private List<string> ConvertStringToListString(object value)
        {
            if(value == null) return new List<string>();
            List<string> stringList = new List<string>();
            foreach(char c in value.ToString()) stringList.Add(c.ToString());
            return stringList;

        }
    }
}
