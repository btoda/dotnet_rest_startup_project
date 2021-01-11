using System.Collections.Generic;
using Data;
using DataService.DataContracts;

namespace DataService.Repositories
{
    public interface IRepository<T> where T : Data.Models.Metadata
    {
        ItemData<T> Find(int id);

        ItemData<T> Find(int id, string includes);

        ItemData<T> Update(T entity);

        ItemData<T> Create(T entity);

        ItemData<T> Delete(int id);

        ItemData<List<T>> GetAll();

        PagedData<T> GetAll(int pageSize, int pageIndex);

        ItemData<List<T>> GetAll(GenericQuerySelectors selectors);

        PagedData<T> GetAll(GenericQuerySelectors selectors, int pageSize, int pageIndex);

        ItemData<CustomQueryResponse> ExecuteRawQuery(string sqlQuery);

        MyDbContext GetContext();

    }
}