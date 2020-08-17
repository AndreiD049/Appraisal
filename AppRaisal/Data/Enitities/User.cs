using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppRaisal.Data.Enitities
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string FullName { get; set; }
        public List<AppraisalItem> Items { get; set; }
    }
}
