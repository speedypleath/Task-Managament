using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManagement.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<UserProject> Users { get; set; }
    }
}