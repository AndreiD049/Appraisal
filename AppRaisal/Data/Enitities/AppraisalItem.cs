using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AppRaisal.Data.Enitities
{
    public class AppraisalItem
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Content { get; set; }
        public int? RelatedId { get; set; }
        public AppraisalItem Related { get; set; }
        public int? AppraisalPeriodId { get; set; }
        public AppraisalPeriod AppraisalPeriod { get; set; }
        public string UserName { get; set; }
        [ForeignKey("UserName")]
        public User User { get; set; }
    }
}
