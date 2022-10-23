using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pairs.Models
{
    public class PairHistory
    {
        public int Day { get; set; }
        public List<string> Pairs { get; set; }
        public List<string> InnovationMembers { get; set; }
        public DateTime Date { get; set; }

    }
}
