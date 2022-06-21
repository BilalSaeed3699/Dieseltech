using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dieseltech.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Comments { get; set; }
        public int Userid { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string CarrierAssignID { get; set; }
        public string Date { get; set; }
    }
}