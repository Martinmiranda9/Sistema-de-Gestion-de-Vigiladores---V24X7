using SGV.Business.Interfaces;
using SGV.Business.Interfaces.Repositories;
using SGV.DTOs.ShiftRecord;
using SGV.DTOs.Workplace;
using SGV.Entities;

namespace SGV.Business.Services;

public class ShiftRecordService : IShiftRecordService
{
    private readonly IShiftRecordRepository _repo;
    private readonly ISecurityGuardRepository _guardRepo;
    private readonly IHolidayRepository _holidayRepo;
    private readonly IWorkplaceRepository _workplaceRepo;

    public ShiftRecordService(
        IShiftRecordRepository repo,
        ISecurityGuardRepository guardRepo,
        IHolidayRepository holidayRepo,
        IWorkplaceRepository workplaceRepo)
    {
        _repo = repo;
        _guardRepo = guardRepo;
        _holidayRepo = holidayRepo;
        _workplaceRepo = workplaceRepo;
    }

    public async Task<IEnumerable<ShiftRecordDTO>> GetAllAsync()
    {
        var records = await _repo.GetAllAsync();
        return records.Select(MapToDTO);
    }

    public async Task<ShiftRecordDTO?> GetByIdAsync(int id)
    {
        var record = await _repo.GetByIdAsync(id);
        return record == null ? null : MapToDTO(record);
    }

    public async Task<ShiftRecordDTO> CreateAsync(ShiftRecordCreateDTO dto)
    {
        // Auto-populate WorkplaceId from the guard's current workplace
        var guard = await _guardRepo.GetByIdAsync(dto.SecurityGuardId);

        var record = new ShiftRecord
        {
            SecurityGuardId = dto.SecurityGuardId,
            WorkplaceId = guard?.WorkplaceId,   // auto-populated
            Date = dto.Date.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Notes = dto.Notes
        };

        await _repo.AddAsync(record);
        await _repo.SaveChangesAsync();

        var created = await _repo.GetByIdAsync(record.Id);
        return MapToDTO(created!);
    }

    public async Task<ShiftRecordDTO?> UpdateAsync(int id, ShiftRecordUpdateDTO dto)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record == null) return null;

        record.SecurityGuardId = dto.SecurityGuardId;
        record.Date = dto.Date.Date;
        record.StartTime = dto.StartTime;
        record.EndTime = dto.EndTime;
        record.Notes = dto.Notes;
        // WorkplaceId is not updated — it's set at creation time

        await _repo.SaveChangesAsync();
        return MapToDTO(record);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record == null) return false;

        _repo.Remove(record);
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ShiftRecordDTO>> GetBySecurityGuardAsync(int securityGuardId, int month, int year)
    {
        var records = await _repo.GetBySecurityGuardAsync(securityGuardId, month, year);
        return records.Select(MapToDTO);
    }

    // ── Summary / Totalizador ──────────────────────────────────────────────────

    public async Task<ShiftSummaryDTO?> GetSummaryAsync(int securityGuardId, int month, int year)
    {
        var guard = await _guardRepo.GetByIdAsync(securityGuardId);
        if (guard == null) return null;

        var records = (await _repo.GetBySecurityGuardAsync(securityGuardId, month, year)).ToList();
        var holidays = (await _holidayRepo.GetByYearAsync(year)).ToList();

        double totalHours = 0;
        double nightHours = 0;
        double holidayHours = 0;

        foreach (var r in records)
        {
            double shiftHours = CalculateShiftHours(r.StartTime, r.EndTime);
            double shiftNight = CalculateNightHours(r.StartTime, r.EndTime);
            bool isHoliday = IsHolidayDate(r.Date, holidays);

            totalHours += shiftHours;
            nightHours += shiftNight;
            if (isHoliday) holidayHours += shiftHours;
        }

        return new ShiftSummaryDTO
        {
            SecurityGuardId = securityGuardId,
            SecurityGuardFullName = $"{guard.LastName}, {guard.FirstName}",
            Month = month,
            Year = year,
            TotalShifts = records.Count,
            TotalHours = Math.Round(totalHours, 2),
            NightHours = Math.Round(nightHours, 2),
            HolidayHours = Math.Round(holidayHours, 2),
            NormalHours = Math.Round(totalHours - nightHours - holidayHours, 2)
        };
    }

    // ── Workplace Calendar / Almanaque ─────────────────────────────────────────

    public async Task<WorkplaceCalendarDTO?> GetWorkplaceCalendarAsync(int workplaceId, int month, int year)
    {
        var workplace = await _workplaceRepo.GetByIdAsync(workplaceId);
        if (workplace == null) return null;

        var records = (await _repo.GetByWorkplaceAsync(workplaceId, month, year)).ToList();
        var holidays = (await _holidayRepo.GetByYearAsync(year)).ToList();

        int daysInMonth = DateTime.DaysInMonth(year, month);

        var days = Enumerable.Range(1, daysInMonth).Select(day =>
        {
            var date = new DateTime(year, month, day);
            var shiftsOnDay = records
                .Where(r => r.Date.Date == date.Date)
                .Select(r => new CalendarShiftDTO
                {
                    ShiftRecordId = r.Id,
                    SecurityGuardId = r.SecurityGuardId,
                    SecurityGuardFullName = $"{r.SecurityGuard.LastName}, {r.SecurityGuard.FirstName}",
                    StartTime = r.StartTime,
                    EndTime = r.EndTime
                }).ToList();

            return new CalendarDayDTO
            {
                Date = date,
                IsHoliday = IsHolidayDate(date, holidays),
                Shifts = shiftsOnDay
            };
        }).ToList();

        return new WorkplaceCalendarDTO
        {
            WorkplaceId = workplaceId,
            WorkplaceName = workplace.Name,
            Month = month,
            Year = year,
            Days = days
        };
    }

    // ── Hour Calculation Helpers ───────────────────────────────────────────────

    private static double CalculateShiftHours(TimeSpan start, TimeSpan end)
    {
        // Overnight shifts: end < start (e.g. 22:00 → 06:00)
        double hours = end > start
            ? (end - start).TotalHours
            : (TimeSpan.FromHours(24) - start + end).TotalHours;
        return hours;
    }

    private static double CalculateNightHours(TimeSpan start, TimeSpan end)
    {
        // Night window: 21:00-24:00 and 00:00-06:00
        var night1Start = TimeSpan.FromHours(21);
        var night1End = TimeSpan.FromHours(24);
        var night2Start = TimeSpan.Zero;
        var night2End = TimeSpan.FromHours(6);

        bool isOvernight = end < start;

        if (!isOvernight)
        {
            // Same-day shift
            return Overlap(start, end, night1Start, night1End)
                 + Overlap(start, end, night2Start, night2End);
        }
        else
        {
            // Overnight: first leg [start, 24:00], second leg [00:00, end]
            return Overlap(start, night1End, night1Start, night1End)
                 + Overlap(night2Start, end, night2Start, night2End);
        }
    }

    private static double Overlap(TimeSpan s1, TimeSpan e1, TimeSpan s2, TimeSpan e2)
    {
        var overlapStart = s1 > s2 ? s1 : s2;
        var overlapEnd = e1 < e2 ? e1 : e2;
        return overlapEnd > overlapStart ? (overlapEnd - overlapStart).TotalHours : 0;
    }

    private static bool IsHolidayDate(DateTime date, List<Holiday> holidays)
    {
        return holidays.Any(h =>
            h.Date.Date == date.Date ||
            (h.IsRecurring && h.Date.Month == date.Month && h.Date.Day == date.Day));
    }

    // ── Mapping ────────────────────────────────────────────────────────────────

    private static ShiftRecordDTO MapToDTO(ShiftRecord r) => new()
    {
        Id = r.Id,
        SecurityGuardId = r.SecurityGuardId,
        SecurityGuardFullName = $"{r.SecurityGuard.LastName}, {r.SecurityGuard.FirstName}",
        Date = r.Date,
        StartTime = r.StartTime,
        EndTime = r.EndTime,
        Notes = r.Notes
    };
}
