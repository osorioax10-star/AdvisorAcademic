using SmartAcademicAdvisor.Core.Interfaces;
using System.Text.Json;

namespace SmartAcademicAdvisor.Services;

public class JsonRepository<T> : IRepository<T> where T : class
{
    private readonly string _filePath;

    public JsonRepository(IWebHostEnvironment env, string fileName)
    {
        _filePath = Path.Combine(env.ContentRootPath, "Data", fileName);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        if (!File.Exists(_filePath))
            return new List<T>();

        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var all = await GetAllAsync();
        var idProp = typeof(T).GetProperty("Id");
        return all.FirstOrDefault(item => idProp?.GetValue(item)?.ToString() == id);
    }

    public async Task AddAsync(T entity)
    {
        var all = (await GetAllAsync()).ToList();
        all.Add(entity);
        await SaveAllAsync(all);
    }

    public async Task UpdateAsync(T entity)
    {
        var all = (await GetAllAsync()).ToList();
        var idProp = typeof(T).GetProperty("Id");
        var id = idProp?.GetValue(entity)?.ToString();
        var index = all.FindIndex(item => idProp?.GetValue(item)?.ToString() == id);
        if (index >= 0)
            all[index] = entity;
        await SaveAllAsync(all);
    }

    public async Task DeleteAsync(string id)
    {
        var all = (await GetAllAsync()).ToList();
        var idProp = typeof(T).GetProperty("Id");
        all.RemoveAll(item => idProp?.GetValue(item)?.ToString() == id);
        await SaveAllAsync(all);
    }

    private async Task SaveAllAsync(IEnumerable<T> items)
    {
        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);
    }
}
