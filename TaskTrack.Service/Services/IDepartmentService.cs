using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public interface IDepartmentService
{
    System.Threading.Tasks.Task<IEnumerable<DepartmentDto>> GetAllActiveAsync();
    System.Threading.Tasks.Task<DepartmentDto?> GetByIdAsync(int id);
    System.Threading.Tasks.Task<IEnumerable<DepartmentDto>> SearchByNameAsync(string name);
    System.Threading.Tasks.Task<DepartmentDto> CreateAsync(DepartmentCreateDto dto);
    System.Threading.Tasks.Task<DepartmentDto?> UpdateAsync(int id, DepartmentUpdateDto dto);
    System.Threading.Tasks.Task<(bool success, string? error)> DeleteAsync(int id);
    System.Threading.Tasks.Task<SummaryDto> GetSummaryAsync();
}
