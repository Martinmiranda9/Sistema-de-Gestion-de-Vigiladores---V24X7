using SGV.Business.Interfaces;
using SGV.Business.Interfaces.Repositories;
using SGV.DTOs.ShiftRecord;
using SGV.Entities;

namespace SGV.Business.Services;

public class ShiftRecordService : IShiftRecordService
{
    private readonly IShiftRecordRepository _repo;

    public ShiftRecordService(IShiftRecordRepository repo) => _repo = repo;

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
        var record = new ShiftRecord
        {
            SecurityGuardId = dto.SecurityGuardId,
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
