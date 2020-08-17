using System;
using System.Collections.Generic;
using System.Text;

namespace AppRaisal.Data.Enitities
{
    public class AppraisalPeriod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public List<AppraisalItem> Items { get; set; }
    }
}
