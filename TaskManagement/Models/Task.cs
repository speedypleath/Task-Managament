using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManagement.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public int ProjectId { get; set; }
        public int StatusId { get; set; }
        public virtual Project Project { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<UserTask> Users { get; set; }
    }
}