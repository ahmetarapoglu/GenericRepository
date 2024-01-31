using GenericRepository.Abstract;
using GenericRepository.Entities;
using GenericRepository.Models.DataTableRequests;
using GenericRepository.Models.Product;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace GenericRepository.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(
        IRepository<Product> productRepository ,
        IRepository<ProductCategory> productCategoryRepository) : ControllerBase
    {
        private readonly IRepository<Product> _productRepository = productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository = productCategoryRepository;

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetProducts(ProductRequest model)
        {
            try
            {

                //Where.
                Expression<Func<Product, bool>> filter = i => true;

                //Date(Filter).
                if (model.StartDate != null)
                    filter = filter.And(i => i.CreatedDate.Date >= model.StartDate.Value.Date);

                if (model.EndDate != null)
                    filter = filter.And(i => i.CreatedDate.Date <= model.EndDate.Value.Date);

                //Search.
                if (!string.IsNullOrEmpty(model.Search))
                    filter = filter.And(i => i.Name.Contains(model.Search));

                //Sort.
                Expression<Func<Product, object>> Order = model.Order switch
                {
                    "Id" => i => i.Id,
                    "Name" => i => i.Name,
                    "Description" => i => i.Description,
                    "Price" => i => i.Price,
                    "CreatedDate" => i => i.CreatedDate,
                    _ => i => i.Id,
                };

                //OrderBy.
                IOrderedQueryable<Product> orderBy(IQueryable<Product> i)
                   => model.SortDir == "ascend"
                   ? i.OrderBy(Order)
                   : i.OrderByDescending(Order);


                //Select
                static IQueryable<ProductGet> select(IQueryable<Product> query) => query.Select(entity => new ProductGet
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    Price = entity.Price,
                    Categories = entity.ProductCategories.Select(i => new CategoryInProduct
                    {
                        CategoryId = i.CategoryId,
                        CategoryName = i.Category.Name,
                    }).ToList(),
                    CreatedDate = entity.CreatedDate
                });

                var (total, data) = await _productRepository.GetListAndTotalAsync(select, filter, null, orderBy, skip: model.Skip, take: model.Take);

                return Ok(new { data, total });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                //Where
                Expression<Func<Product, bool>> filter = i => i.Id == id;

                //Select
                static IQueryable<ProductGet> select(IQueryable<Product> query) => query.Select(entity => new ProductGet
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    Price = entity.Price,
                    Categories = entity.ProductCategories.Select(i=> new CategoryInProduct
                    {
                        CategoryId = i.CategoryId,
                        CategoryName = i.Category.Name,
                    }).ToList(),
                    CreatedDate = entity.CreatedDate
                });

                var category = await _productRepository.FindAsync(select, filter);

                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateProduct(ProductCreate model)
        {
            try
            {
                var entity = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    ProductCategories = model.Categories.Select(i=> new ProductCategory
                    {
                        CategoryId = i,
                    }).ToList(),
                    CreatedDate = DateTime.Now,
                };

                await _productRepository.AddAsync(entity);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateProduct(ProductUpdate model)
        {
            try
            {
                if (model.Id == 0 || model?.Id == null)
                    throw new Exception("Reauested Product Not Found!.");

                //Where
                Expression<Func<Product, bool>> filter = i => i.Id == model.Id;

                Expression<Func<ProductCategory, bool>> Predicte = i => i.ProductId == model.Id;

                await _productCategoryRepository.DeleteRangeAsync(Predicte);

                //Include.
                IIncludableQueryable<Product, object> include(IQueryable<Product> query) => query
                   .Include(i => i.ProductCategories);

                void action(Product product)
                {
                    product.Name = model.Name;
                    product.Description = model.Description;
                    product.Price = model.Price;
                    product.ProductCategories = model.Categories.Select(i => new ProductCategory
                    {
                        CategoryId = i,
                    }).ToList();
                }

                await _productRepository.UpdateAsync(action, filter , include);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                Expression<Func<Product, bool>> filter = i => i.Id == id;

                await _productRepository.DeleteAsync(filter);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
