using System;
using Aris.Moe.OverlayTranslate.Server.Ocr;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Model
{
    public class VoteModel
    {
        public int For { get; set; }
        public int UserId { get; set; }
        public Vote Value { get; set; }
        public DateTime ChangedOn { get; set; }
    }
}