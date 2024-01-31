using GenericRepository.Abstract;
using GenericRepository.Entities;
using GenericRepository.Models.Category;
using GenericRepository.Models.DataTableRequests;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace GenericRepository.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(IRepository<Category> categoryRepository) : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepository = categoryRepository;

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetCategories(CategoryRequest model)
        {
            try
            {

                //Where.
                Expression<Func<Category, bool>> filter = i => true;

                //Date(Filter).
                if (model.StartDate != null)
                    filter = filter.And(i => i.CreatedDate.Date >= model.StartDate.Value.Date);

                if (model.EndDate != null)
                    filter = filter.And(i => i.CreatedDate.Date <= model.EndDate.Value.Date);

                //Search.
                if (!string.IsNullOrEmpty(model.Search))
                    filter = filter.And(i => i.Name.Contains(model.Search));

                //Sort.
                Expression<Func<Category, object>> Order = model.Order switch
                {
                    "Id" => i => i.Id,
                    "Name" => i => i.Name,
                    "CreateDate" => i => i.CreatedDate,
                    _ => i => i.Id,
                };

                //OrderBy.
                IOrderedQueryable<Category> orderBy(IQueryable<Category> i)
                   => model.SortDir == "ascend"
                   ? i.OrderBy(Order)
                   : i.OrderByDescending(Order);

                //Select.
                static IQueryable<CategoryGet> select(IQueryable<Category> query) => query.Select(entity => new CategoryGet
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    CreatedDate = entity.CreatedDate
                });

                var (total, data) = await _categoryRepository.GetListAndTotalAsync(select, filter, null, orderBy, skip: model.Skip, take: model.Take);

                return Ok(new { data, total });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                //Where
                Expression<Func<Category, bool>> filter = i => i.Id == id;

                //Select
                static IQueryable<CategoryGet> select(IQueryable<Category> query) => query.Select(entity => new CategoryGet
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    CreatedDate = entity.CreatedDate
                });

                var category = await _categoryRepository.FindAsync(select, filter);

                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateCategory(CategoryCreate model)
        {
            try
            {
                var entity = new Category
                {
                    Name = model.Name,
                    CreatedDate = DateTime.Now,
                };

                await _categoryRepository.AddAsync(entity);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateCategory(CategoryUpdate model)
        {
            try
            {
                if (model.Id == 0 || model?.Id == null)
                    throw new Exception("Reauested Category Not Found!.");

                //Where
                Expression<Func<Category, bool>> filter = i => i.Id == model.Id;

                void action(Category category)
                {
                    category.Name = model.Name;
                }

                await _categoryRepository.UpdateAsync(action, filter);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                Expression<Func<Category, bool>> filter = i => i.Id == id;

                await _categoryRepository.DeleteAsync(filter);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
