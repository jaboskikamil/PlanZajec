using System.ComponentModel.DataAnnotations;

namespace UniversityTimetable.Models;

public class Lecturer
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty; 
    
    public string FullName => $"{Title} {FirstName} {LastName}".Trim();
    
    public ICollection<TimetableEntry> TimetableEntries { get; set; } = new List<TimetableEntry>();
}

public class Group
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty; 
    public int Year { get; set; }
    public int Semester { get; set; }
    
    public ICollection<TimetableEntry> TimetableEntries { get; set; } = new List<TimetableEntry>();
}

public enum DayOfWeek
{
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7
}

public class TimetableEntry
{
    public int Id { get; set; }
    [Required]
    public string SubjectName { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; 
    
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    public int LecturerId { get; set; }
    public Lecturer Lecturer { get; set; } = null!;
    
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;
}
