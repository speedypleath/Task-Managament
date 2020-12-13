using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaskManagement.Models
{
    public class UserProject
    {
        public int ProjectId { get; set; }
        public Task Project { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsEditor { get; set; }
    }

}