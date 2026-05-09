using Microsoft.EntityFrameworkCore;
using UniversityTimetable.Data;
using UniversityTimetable.Models;

namespace UniversityTimetable.Services;

public class TimetableService
{
    private readonly IDbContextFactory<TimetableContext> _contextFactory;

    public TimetableService(IDbContextFactory<TimetableContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Lecturer>> GetLecturersAsync(string? searchTerm = null)
    {
        using var context = _contextFactory.CreateDbContext();
        var query = context.Lecturers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(l => 
                l.FirstName.ToLower().Contains(searchTerm) || 
                l.LastName.ToLower().Contains(searchTerm));
        }

        return await query.OrderBy(l => l.LastName).ToListAsync();
    }

    public async Task<List<int>> GetYearsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Groups
            .Select(g => g.Year)
            .Distinct()
            .OrderBy(y => y)
            .ToListAsync();
    }

    public async Task<List<int>> GetSemestersAsync(int year)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Groups
            .Where(g => g.Year == year)
            .Select(g => g.Semester)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();
    }

    public async Task<List<Group>> GetGroupsAsync(int year, int semester)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Groups
            .Where(g => g.Year == year && g.Semester == semester)
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<List<TimetableEntry>> GetTimetableForLecturerAsync(int lecturerId, Models.DayOfWeek day)
    {
        using var context = _contextFactory.CreateDbContext();
        var entries = await context.TimetableEntries
            .Include(e => e.Group)
            .Include(e => e.Lecturer)
            .Where(e => e.LecturerId == lecturerId && e.Day == day)
            .ToListAsync();
        
        return entries.OrderBy(e => e.StartTime).ToList();
    }

    public async Task<List<TimetableEntry>> GetTimetableForGroupAsync(int groupId, Models.DayOfWeek day)
    {
        using var context = _contextFactory.CreateDbContext();
        var entries = await context.TimetableEntries
            .Include(e => e.Group)
            .Include(e => e.Lecturer)
            .Where(e => e.GroupId == groupId && e.Day == day)
            .ToListAsync();

        return entries.OrderBy(e => e.StartTime).ToList();
    }

    public async Task<Lecturer?> GetLecturerByIdAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Lecturers.FindAsync(id);
    }

    public async Task<Group?> GetGroupByIdAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Groups.FindAsync(id);
    }
}
