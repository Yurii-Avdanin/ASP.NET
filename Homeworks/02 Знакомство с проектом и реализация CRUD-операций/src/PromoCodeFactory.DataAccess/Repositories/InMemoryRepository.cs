using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Exceptions;
using System.Collections.Concurrent;

namespace PromoCodeFactory.DataAccess.Repositories;

public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ConcurrentDictionary<Guid, T> _data;

    public InMemoryRepository(IEnumerable<T> data)
    {
        _data = new ConcurrentDictionary<Guid, T>(data.Select(e => new KeyValuePair<Guid, T>(e.Id, e)));
    }
    public Task<IReadOnlyCollection<T>> GetAll(CancellationToken ct)
    {        
        return Task.FromResult((IReadOnlyCollection<T>)_data.Values);
    }

    public Task<T?> GetById(Guid id, CancellationToken ct)
    {
        return _data.TryGetValue(id, out var result) ?
            Task.FromResult((T?)result) :
            Task.FromResult((T?)null);
    }

    public Task Add(T entity, CancellationToken ct)
    {
        if (_data.TryAdd(entity.Id, entity))
            return Task.CompletedTask;
        
        throw new InvalidOperationException($"Не удалось добавить элемент типа \"{typeof(T).Name}\" с Id {entity.Id}");
    }

    public Task Update(T entity, CancellationToken ct)
    {
        if (_data.ContainsKey(entity.Id))
        {
            if (_data.TryUpdate(entity.Id, entity, _data[entity.Id]))            
                return Task.CompletedTask;

            throw new InvalidOperationException($"Не удалось обновить элемент типа \"{typeof(T).Name}\" с Id {entity.Id}");
        }

        throw new EntityNotFoundException(entity.GetType(), entity.Id);
    }

    public Task Delete(Guid id, CancellationToken ct)
    {        
        if (_data.TryRemove(id, out _))        
            return Task.CompletedTask;
     
        throw new EntityNotFoundException(typeof(T), id);
    }
}
