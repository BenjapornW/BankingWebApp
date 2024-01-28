using System;
namespace McbaAdminWebAPI.Models.Repository;

public interface IDataRepository<TEntity, TKey> where TEntity : class
{
    IEnumerable<TEntity> GetAll();
    TEntity Get(TKey id);
    TKey Add(TEntity item);
    TKey Update(TKey id, TEntity item);
    TKey Delete(TKey id);
}

// reference lecture Day 9

