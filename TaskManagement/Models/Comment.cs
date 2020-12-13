﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManagement.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int TaskId { get; set; }
        public virtual Task Task { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}