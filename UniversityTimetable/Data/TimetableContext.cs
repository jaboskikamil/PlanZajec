using Microsoft.EntityFrameworkCore;
using UniversityTimetable.Models;

namespace UniversityTimetable.Data;

public class TimetableContext : DbContext
{
    public TimetableContext(DbContextOptions<TimetableContext> options) : base(options) { }

    public DbSet<Lecturer> Lecturers { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<TimetableEntry> TimetableEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<TimetableEntry>()
            .HasOne(e => e.Lecturer)
            .WithMany(l => l.TimetableEntries)
            .HasForeignKey(e => e.LecturerId);

        modelBuilder.Entity<TimetableEntry>()
            .HasOne(e => e.Group)
            .WithMany(g => g.TimetableEntries)
            .HasForeignKey(e => e.GroupId);
    }
}
