using Microsoft.EntityFrameworkCore;
using UniversityTimetable.Models;

namespace UniversityTimetable.Data;

public static class DbInitializer
{
    public static void Initialize(TimetableContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

      
        var lecturers = new[]
        {
            new Lecturer { FirstName = "Jan", LastName = "Kowalski", Title = "Dr inż." },
            new Lecturer { FirstName = "Anna", LastName = "Nowak", Title = "Prof. dr hab." },
            new Lecturer { FirstName = "Piotr", LastName = "Zieliński", Title = "Mgr" },
            new Lecturer { FirstName = "Marta", LastName = "Wiśniewska", Title = "Dr" },
            new Lecturer { FirstName = "Robert", LastName = "Lewandowski", Title = "Mgr inż." },
            new Lecturer { FirstName = "Tomasz", LastName = "Kaczmarek", Title = "Dr" },
            new Lecturer { FirstName = "Ewa", LastName = "Dąbrowska", Title = "Dr hab." },
            new Lecturer { FirstName = "Krzysztof", LastName = "Mazur", Title = "Mgr" },
            new Lecturer { FirstName = "Agnieszka", LastName = "Piotrowska", Title = "Dr" },
            new Lecturer { FirstName = "Paweł", LastName = "Wójcik", Title = "Dr inż." }
        };

        context.Lecturers.AddRange(lecturers);
        context.SaveChanges();

        
        var groups = new[]
        {
            new Group { Name = "G1.A", Year = 1, Semester = 1 },
            new Group { Name = "G1.B", Year = 1, Semester = 1 },
            new Group { Name = "G2.A", Year = 1, Semester = 2 },
            new Group { Name = "G2.B", Year = 1, Semester = 2 },
            new Group { Name = "G3", Year = 2, Semester = 3 },
            new Group { Name = "G4", Year = 2, Semester = 4 }
        };

        context.Groups.AddRange(groups);
        context.SaveChanges();

       
        var lecturerSubjects = new Dictionary<int, List<string>>
        {
            { lecturers[0].Id, new List<string> { "Programowanie" } },
            { lecturers[1].Id, new List<string> { "Matematyka" } },
            { lecturers[2].Id, new List<string> { "Bazy Danych" } },
            { lecturers[3].Id, new List<string> { "Fizyka" } },
            { lecturers[4].Id, new List<string> { "Sieci Komputerowe" } },

            { lecturers[5].Id, new List<string> { "Programowanie" } },
            { lecturers[6].Id, new List<string> { "Matematyka", "Statystyka" } },
            { lecturers[7].Id, new List<string> { "Systemy Operacyjne" } },
            { lecturers[8].Id, new List<string> { "Algorytmy" } },
            { lecturers[9].Id, new List<string> { "Język Angielski" } }
        };

        var types = new[] { "Wykład", "Ćwiczenia", "Laboratorium" };

        var days = new[]
        {
            Models.DayOfWeek.Monday,
            Models.DayOfWeek.Tuesday,
            Models.DayOfWeek.Wednesday,
            Models.DayOfWeek.Thursday,
            Models.DayOfWeek.Friday
        };

        var random = new Random();

        var lecturerSchedule = new Dictionary<int, List<(Models.DayOfWeek, TimeSpan, TimeSpan)>>();

        foreach (var lec in lecturers)
        {
            lecturerSchedule[lec.Id] = new List<(Models.DayOfWeek, TimeSpan, TimeSpan)>();
        }

        bool IsLecturerAvailable(int lecturerId, Models.DayOfWeek day, TimeSpan start, TimeSpan end)
        {
            return !lecturerSchedule[lecturerId].Any(s =>
                s.Item1 == day &&
                !(end <= s.Item2 || start >= s.Item3)
            );
        }

        var entries = new List<TimetableEntry>();

        foreach (var group in groups)
        {
            foreach (var day in days)
            {
                var start = new TimeSpan(8, 0, 0);
                int lessonsCount = random.Next(2, 5); 

                for (int i = 0; i < lessonsCount; i++)
                {
                    var end = start.Add(TimeSpan.FromMinutes(90));

                    Lecturer chosenLecturer = null;
                    string subject = null;

                    foreach (var lec in lecturers.OrderBy(x => random.Next()))
                    {
                        if (!IsLecturerAvailable(lec.Id, day, start, end))
                            continue;

                        var subjects = lecturerSubjects[lec.Id];
                        subject = subjects[random.Next(subjects.Count)];
                        chosenLecturer = lec;
                        break;
                    }

                    if (chosenLecturer == null)
                    {
                        start = start.Add(TimeSpan.FromMinutes(30));
                        i--;
                        continue;
                    }

                    entries.Add(new TimetableEntry
                    {
                        SubjectName = subject,
                        Room = $"S{random.Next(100, 400)}",
                        Type = types[random.Next(types.Length)],
                        Day = day,
                        StartTime = start,
                        EndTime = end,
                        LecturerId = chosenLecturer.Id,
                        GroupId = group.Id
                    });

                    lecturerSchedule[chosenLecturer.Id].Add((day, start, end));

                    var breakMinutes = new[] { 15, 30, 45 }[random.Next(3)];
                    start = end.Add(TimeSpan.FromMinutes(breakMinutes));
                }
            }
        }

        context.TimetableEntries.AddRange(entries);
        context.SaveChanges();
    }
}