using System;
using Data.Models;
using DataService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DataService.Controllers
{
    public class ProductController : GenericController<Product>
    {
        public ProductController(IRepository<Product> repository) : base(repository)
        {

        }
        [HttpGet]
        [Route("[controller]/MyRandomInt")]
        public int MyRandomInt()
        {
            return new Random().Next(100);
        }
    }
}