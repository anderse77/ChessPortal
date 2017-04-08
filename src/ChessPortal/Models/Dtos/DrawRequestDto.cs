using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPortal.Models.Dtos
{
    public class DrawRequestDto
    {
        public Guid ChallengeId { get; set; }
        public string PlayerId { get; set; }
    }
}
