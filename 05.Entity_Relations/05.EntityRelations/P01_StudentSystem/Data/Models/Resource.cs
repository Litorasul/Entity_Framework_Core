using System.ComponentModel.DataAnnotations;

namespace P01_StudentSystem.Data.Models
{
    public enum ResourceOption
    {
        Video = 1,
        Presentation = 2,
        Document = 3,
        Other = 4
    }
    public class Resource
    {
        public int ResourceId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        
        [Required]
        public string Url { get; set; }

        [Required]
        public ResourceOption ResourceType { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }


    }
}
