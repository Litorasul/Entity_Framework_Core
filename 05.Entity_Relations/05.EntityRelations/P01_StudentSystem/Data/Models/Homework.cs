using System;
using System.ComponentModel.DataAnnotations;

namespace P01_StudentSystem.Data.Models
{
    public enum ContentTypeOptions
    {
        Application = 1, 
        Pdf = 2,
        Zip = 3
    }
    public class Homework
    {
        public int HomeworkId { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        [Required]
        public ContentTypeOptions ContentType { get; set; }
        
        [Required]
        public DateTime SubmissionTime { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }


        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}
