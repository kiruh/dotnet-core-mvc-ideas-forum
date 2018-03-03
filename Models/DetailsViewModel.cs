using System.ComponentModel.DataAnnotations;
using Ideas.Models;

namespace Ideas.Models
{
    public class DetailsViewModel
    {
        public Idea Idea { get; set; }
        public Comment NewComment { get; set; }
    }
}