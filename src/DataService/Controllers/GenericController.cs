using System;
using Microsoft.AspNetCore.Mvc;
using DataService.DataContracts;
using System.Collections.Generic;
using System.Reflection;
using DataService.Repositories;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Buffers;
using DataService.Serializer;
using Microsoft.AspNetCore.Mvc.Formatters;
using DataService.Features;
using System.Linq;

namespace DataService
{
    public class GenericController<T> : Controller where T : Data.Models.Metadata
    {

        protected IRepository<T> repo = null;
        private Dictionary<Type, string[]> RegisteredTypeFieldFilter = new Dictionary<Type, string[]>();

        public GenericController(IRepository<T> repository) : base()
        {
            this.repo = repository;
        }


        [HttpGet("[controller]/")]
        public virtual ItemData<List<T>> GetAll()
        {
            try
            {
                return repo.GetAll();
            }
            catch (Exception ex)
            {
                ItemData<List<T>> result = new ItemData<List<T>>();
                result.Error = new DataContracts.Error(ex);
                return result;
            }
        }

        [HttpPost("[controller]/QuerySelector")]
        public virtual ItemData<List<T>> GetAllQuerySelector([FromBody] GenericQuerySelectors selectors)
        {
            try
            {
                return repo.GetAll(selectors);
            }
            catch (Exception ex)
            {
                ItemData<List<T>> result = new ItemData<List<T>>();
                result.Error = new DataContracts.Error(ex);
                return result;
            }
        }

        [HttpGet("[controller]/Paged")]
        public virtual PagedData<T> GetAllPaged([FromQuery] int pageSize = 20, [FromQuery] int pageIndex = 1)
        {
            try
            {
                return repo.GetAll(pageSize, pageIndex);
            }
            catch (Exception ex)
            {
                PagedData<T> result = new PagedData<T>();
                result.Error = new DataContracts.Error(ex);
                return result;
            }
        }

        [HttpPost("[controller]/PagedQuerySelector")]
        public virtual PagedData<T> GetAllPagedQuerySelector([FromBody] GenericQuerySelectors selectors, [FromQuery] int pageSize = 20, [FromQuery] int pageIndex = 1)
        {
            try
            {
                return repo.GetAll(selectors, pageSize, pageIndex);
            }
            catch (Exception ex)
            {
                PagedData<T> result = new PagedData<T>();
                result.Error = new DataContracts.Error(ex);
                return result;
            }
        }

        [HttpGet("[controller]/{id}")]
        public virtual ItemData<T> Find(int id, [FromQuery] string includes)
        {
            try
            {
                return repo.Find(id, includes);
            }
            catch (Exception ex)
            {
                ItemData<T> result = new ItemData<T>();
                result.Error = new DataContracts.Error(ex);
                return result;
            }
        }

        [HttpPost("[controller]")]
        public virtual ItemData<T> Create([FromBody] T item)
        {
            PropertyInfo pinfo = item.GetType().GetProperty("Id");
            int idValue = 0;
            if (pinfo != null)
            {
                idValue = (int)pinfo.GetValue(item);
            }
            if (idValue > 0)
            {
                ItemData<T> result = new ItemData<T>();
                result.Error = new DataContracts.Error("Could not vreate item.");
                return result;
            }
            try
            {
                item.DateCreated = DateTime.Now;
                return repo.Create(item);
            }
            catch (Exception ex)
            {
                ItemData<T> result = new ItemData<T>();
                result.Error = new DataContracts.Error(ex);
                return result;
            }
        }

        [HttpDelete("[controller]/{id}")]
        public virtual ItemData<T> Delete(int id)
        {
            ItemData<T> result = new ItemData<T>();
            try
            {
                return repo.Delete(id);
            }
            catch (Exception ex)
            {
                result.Error = new DataContracts.Error(ex);
                return result;
            }
        }

        [HttpPut("[controller]")]
        public virtual ItemData<T> Update([FromBody] T item)
        {
            try
            {
                item.DateModified = DateTime.Now;
                return repo.Update(item);
            }
            catch (Exception ex)
            {
                ItemData<T> result = new ItemData<T>();
                result.Error = new DataContracts.Error(ex);
                return result;
            }
        }

        protected bool RegisterTypeFieldFilter(Type type, string[] fields)
        {
            if (fields == null || fields.Length == 0)
            {
                return false;
            }
            if (RegisteredTypeFieldFilter.ContainsKey(type))
            {
                RegisteredTypeFieldFilter[type] = fields;
            }
            else
            {
                RegisteredTypeFieldFilter.Add(type, fields);
            }
            return true;
        }

        protected string[] GetFieldsFilter()
        {
            var fieldHeader = this.HttpContext.Request.Headers["DataFields"];
            if (fieldHeader.Count == 1)
            {
                var result = fieldHeader.ToString().Split(',').Select(a => a.Trim()).Where(a => !string.IsNullOrEmpty(a)).ToArray();
                return result.Count() > 0 ? result : null;
            }
            return null;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            RegisterTypeFieldFilter(typeof(T), GetFieldsFilter());
            if (context.Result is ObjectResult objectResult)
            {
                var result = context.Result as ObjectResult;
                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new FilterFieldsContractResolver(RegisteredTypeFieldFilter),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                result.Formatters.Add(new NewtonsoftJsonOutputFormatter(settings, ArrayPool<Char>.Shared, new MvcOptions()));
            }
            base.OnActionExecuted(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // get the current action name
            var actionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ActionName;
            // get the visibility settings configured in the startup class, or default them to all
            var actionsVisibility = Features.EntityTypes.ActionVisibility.ContainsKey(typeof(T).FullName) ?
                    Features.EntityTypes.ActionVisibility[typeof(T).FullName] : GenericControllerActionVisibility.All;

            // by convention the name of the method and the name of the visibility enum match
            // so we try to parse the name of the method in the visibility enum so we can compare afterwards
            GenericControllerActionVisibility currentActionVisibilityCheck = GenericControllerActionVisibility.All;
            Enum.TryParse(actionName, true, out currentActionVisibilityCheck);

            // this practically sais, that if this type is not marked with all or with an enum visibility
            // with the same name as the current method, than we return 404
            if (actionsVisibility != GenericControllerActionVisibility.All &&
            currentActionVisibilityCheck != GenericControllerActionVisibility.All &&
            (currentActionVisibilityCheck & actionsVisibility) == 0)
            {
                context.Result = new StatusCodeResult(404);
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}