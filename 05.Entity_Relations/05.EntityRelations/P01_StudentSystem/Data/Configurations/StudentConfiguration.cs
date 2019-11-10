using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P01_StudentSystem.Data.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> student)
        {
            student
                .Property(p => p.PhoneNumber)
                .IsUnicode(false)
                .HasMaxLength(10)
                .IsFixedLength(true);

            student
                .HasMany(s => s.HomeworkSubmissions)
                .WithOne(h => h.Student)
                .HasForeignKey(h => h.StudentId);
        }
    }
}
