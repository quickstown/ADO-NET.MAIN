using Microsoft.EntityFrameworkCore;
using College_DB.Models;

namespace CheToNovoe.Data
{
    public class CollegeDBContext : DbContext
    {
        public CollegeDBContext(DbContextOptions<CollegeDBContext> options)
            : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Subjects> Subjects => Set<Subjects>();
        public DbSet<Grades> Grades => Set<Grades>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(e =>
            {
                e.ToTable("Students");
                e.HasKey(s => s.StudentId);
                e.Property(s => s.FirstName).HasMaxLength(50).IsRequired();
                e.Property(s => s.LastName).HasMaxLength(50).IsRequired();

                e.HasOne(s => s.Group)
                    .WithMany(g => g.Students)
                    .HasForeignKey(s => s.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Group>(e =>
            {
                e.ToTable("Groups");
                e.HasKey(g => g.GroupId);
                e.Property(s => s.GroupName).HasMaxLength(20).IsRequired();
            });

            modelBuilder.Entity<Teacher>(e =>
            {
                e.ToTable("Teachers");
                e.HasKey(t => t.TeacherId);
                e.Property(t => t.FirstName).HasMaxLength(50).IsRequired();
                e.Property(t => t.LastName).HasMaxLength(50).IsRequired();
            });
            modelBuilder.Entity<Subjects>(e =>
            {
                e.ToTable("Subjects");
                e.HasKey(s => s.SubjectsId);
                e.Property(s => s.SubjectsName).HasMaxLength(100).IsRequired();

                e.HasOne(s => s.Teacher)
                    .WithMany(t => t.Subjects)
                    .HasForeignKey(s => s.TeacherId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Grades>(e =>
            {
                e.ToTable("Grades");
                e.HasKey(g => g.GradesId);
                e.Property(g => g.GradeDate).HasColumnType("date");

                e.HasOne(g => g.Student)
                    .WithMany(s => s.Grades)
                    .HasForeignKey(g => g.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(g => g.Subjects)
                    .WithMany(s => s.Grades)
                    .HasForeignKey(g => g.SubjectsId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}