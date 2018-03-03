using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ideas.Models
{
    public class Comment
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Text { get; set; }

        [Required]
        public int IdeaId { get; set; }
        public Idea Idea { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}