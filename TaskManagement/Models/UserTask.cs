using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.Models
{
    public class UserTask
    {
        public int TaskId { get; set; }
        public Task Task { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}