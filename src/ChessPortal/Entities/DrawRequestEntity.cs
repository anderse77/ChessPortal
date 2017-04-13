﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPortal.Entities
{
    public class DrawRequestEntity
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("PlayerId")]
        public ChessPlayer Player { get; set; }
        public string PlayerId { get; set; }
        [ForeignKey("ChallengeId")]
        public ChallengeEntity Challenge { get; set; }
        public Guid ChallengeId { get; set; }
    }
}