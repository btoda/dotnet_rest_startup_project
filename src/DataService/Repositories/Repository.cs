using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Dynamic.Core;
using Data;
using DataService.DataContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DataService.Repositories
{
    public class Repository<T> : IRepository<T> where T : Data.Models.Metadata
    {
        private readonly string connectionString;
        private MyDbContext context = null;
        private DbSet<T> repoCollection = null;

        public Repository(IOptions<Config> config)
        {
            this.connectionString = config.Value.ConnectionString;
        }

        public Repository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public MyDbContext GetContext()
        {
            if (this.context != null)
            {
                return context;
            }
            var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
            context = new MyDbContext(connectionString, optionsBuilder.Options);
            return context;
        }

        private DbSet<T> GetCollection()
        {
            if (this.repoCollection != null)
            {
                return this.repoCollection;
            }
            var context = GetContext();
            var properties = context.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(DbSet<T>))
                {
                    this.repoCollection = (DbSet<T>)property.GetValue(context);
                    return this.repoCollection;
                }
            }
            return null;
        }

        public virtual ItemData<T> Find(int id)
        {
            var entity = (T)GetContext().Find(typeof(T), new object[] { id });
            ItemData<T> result = new ItemData<T>();
            result.Data = entity;
            return result;
        }

        public virtual ItemData<T> Find(int id, string includes)
        {
            var query = GetCollection().AsQueryable();
            if (!string.IsNullOrEmpty(includes))
            {
                string[] includeArray = includes.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string include in includeArray)
                {
                    query = query.Include(include.Trim());
                }
            }
            var entity = query.Where($"Id=@0", new object[] { id }).FirstOrDefault();
            ItemData<T> result = new ItemData<T>();
            result.Data = entity;
            return result;
        }

        public virtual ItemData<T> Update(T entity)
        {
            var context = GetContext();
            context.Update((object)entity);
            context.SaveChanges();
            ItemData<T> result = new ItemData<T>();
            result.Data = entity;
            return result;
        }

        public virtual ItemData<T> Create(T entity)
        {
            var context = GetContext();
            context.Add((object)entity);
            context.SaveChanges();
            ItemData<T> result = new ItemData<T>();
            result.Data = entity;
            return result;
        }

        public virtual ItemData<T> Delete(int id)
        {
            var context = GetContext();
            var entityData = Find(id);
            if (entityData.Error == null)
            {
                context.Remove(entityData.Data);
                context.SaveChanges();
                return entityData;
            }
            else
            {
                return entityData;
            }
        }

        public virtual ItemData<CustomQueryResponse> ExecuteRawQuery(string sqlQuery)
        {
            DbCommand command = GetContext().Database.GetDbConnection().CreateCommand();
            command.CommandText = sqlQuery;
            command.CommandType = CommandType.Text;
            context.Database.OpenConnection();
            ItemData<CustomQueryResponse> response = new ItemData<CustomQueryResponse>();
            response.Data = new CustomQueryResponse();
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            using (var res = command.ExecuteReader())
            {
                var entities = new List<List<dynamic>>();
                List<string> columns = new List<string>();
                response.Data.Columns = columns;
                for (int i = 0; i < res.FieldCount; i++)
                {
                    columns.Add(res.GetName(i));
                }
                while (res.Read())
                {
                    object[] values = new object[res.FieldCount];
                    res.GetValues(values);
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    for (int i = 0; i < values.Length; i++)
                    {
                        item.Add(columns[i], values[i]);
                    }
                    data.Add(item);
                }
            }
            response.Data.Data = data;
            return response;
        }

        public virtual ItemData<List<T>> GetAll()
        {
            ItemData<List<T>> result = new ItemData<List<T>>();
            result.Data = GetCollection().ToList();
            return result;
        }

        public virtual PagedData<T> GetAll(int pageSize, int pageIndex)
        {
            if (pageIndex < 1)
            {
                throw new System.Exception("The required page index must be a positive integer (>=1)");
            }
            PagedData<T> result = new PagedData<T>();
            result.ItemsCount = GetCollection().Count();
            result.PageIndex = pageIndex;
            result.PageSize = pageSize;
            result.Data = GetCollection().Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            return result;
        }

        private IQueryable<T> GetGenericSelectorQuery(GenericQuerySelectors selectors)
        {
            var dbset = GetCollection().AsQueryable();
            if (selectors != null)
            {
                if (selectors.Filters != null)
                {
                    foreach (GenericQueryFilter filter in selectors.Filters)
                    {
                        if (!string.IsNullOrEmpty(filter.OpName))
                        {
                            dbset = dbset.Where($"{filter.FieldName} {filter.OpName} @0", new object[] { filter.FilterValue });
                        }
                        else if (!string.IsNullOrEmpty(filter.MethodName))
                        {
                            dbset = dbset.Where($"{filter.FieldName}.ToLower().{filter.MethodName}(@0)", new object[] { filter.FilterValue.ToLower() });
                        }
                    }
                }
                if (selectors.SortOrders != null)
                {
                    foreach (GenericQuerySortOrder sortOrder in selectors.SortOrders)
                    {
                        if (sortOrder.OrderDirection != null && sortOrder.OrderDirection.ToLower() == "desc")
                        {
                            dbset = dbset.OrderBy($"{sortOrder.FieldName} desc");
                        }
                        else
                        {
                            dbset = dbset.OrderBy($"{sortOrder.FieldName} asc");
                        }
                    }
                }

                if (selectors.Includes != null)
                {
                    foreach (string include in selectors.Includes)
                    {
                        dbset = dbset.Include(include);
                    }
                }
            }
            return dbset;
        }

        public virtual ItemData<List<T>> GetAll(GenericQuerySelectors selectors)
        {
            ItemData<List<T>> result = new ItemData<List<T>>();
            result.Data = GetGenericSelectorQuery(selectors).ToList();
            return result;
        }

        public virtual PagedData<T> GetAll(GenericQuerySelectors selectors, int pageSize, int pageIndex)
        {
            PagedData<T> result = new PagedData<T>();
            var query = GetGenericSelectorQuery(selectors);

            result.ItemsCount = query.Count();
            result.PageIndex = pageIndex;
            result.PageSize = pageSize;
            result.Data = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return result;
        }
    }
}