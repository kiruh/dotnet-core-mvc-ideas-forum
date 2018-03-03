using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ideas.Models
{
    public class Idea
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Text { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool Approved { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}