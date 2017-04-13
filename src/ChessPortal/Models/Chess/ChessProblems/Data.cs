using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessPortal.Models.Chess.ChessProblems
{
    public class Data
    {
        public string BlunderMove { get; set; }
        public int Elo { get; set; }
        public string FenBefore { get; set; }
        public string[] ForcedLine { get; set; }
        public string Id { get; set; }
    }
}
