using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Data;
using MongoDB.Data.Repositories.Interfaces;
using MongoDB.Models;
using MongoDB.Repository.Extensions;
using MongoDB.UnitOfWork;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BlogsController : Controller
    {
        private readonly IMongoDbUnitOfWork<BloggingContext> _unitOfWork;

        public BlogsController(IMongoDbUnitOfWork<BloggingContext> unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork), $"{nameof(unitOfWork)} cannot be null.");
        }

        // GET: api/Blogs
        [HttpGet]
        public async Task<IActionResult> GetBlogs(int? pageIndex = null, int? pageSize = null)
        {
            if (pageIndex <= 0 || pageSize <= 0)
            {
                return BadRequest();
            }

            var repository = _unitOfWork.Repository<Blog>();

            // Example: Paging
            var query = repository.MultipleResultQuery()
                                  .Page(pageIndex, pageSize);

            if (query.Paging.IsEnabled)
            {
                // Example: PagedList
                var blogs = await repository.SearchAsync(query)
                                            .ToPagedListAsync(
                                                query.Paging.PageIndex,
                                                query.Paging.PageSize,
                                                query.Paging.TotalCount)
                                            .ConfigureAwait(continueOnCapturedContext: false);

                return Ok(blogs);
            }
            else
            {
                var blogs = await repository.SearchAsync(query)
                                            .ConfigureAwait(continueOnCapturedContext: false);

                return Ok(blogs);
            }
        }

        // GET: api/Blogs
        [HttpGet("Search", Name = "GetBlogsByTerm")]
        public async Task<IActionResult> GetBlogsByTerm(string term)
        {
            var repository = _unitOfWork.Repository<Blog>();

            // Example: Filtering
            var query = repository.MultipleResultQuery()
                                  .AndFilter(blog => blog.Title.Contains(term));

            var blogs = await repository.SearchAsync(query)
                                        .ConfigureAwait(continueOnCapturedContext: false);

            return Ok(blogs);
        }

        // GET: api/Blogs/Urls
        [HttpGet("Urls")]
        public IActionResult GetUrls()
        {
            // Example: Custom Repository
            var repository = _unitOfWork.CustomRepository<ICustomBlogRepository>();

            var urls = repository.GetAllBlogUrls();

            return Ok(urls);
        }

        // GET: api/Blogs/5
        [HttpGet("{id}", Name = "GetBlogById")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var repository = _unitOfWork.Repository<Blog>();

            var query = repository.SingleResultQuery()
                                  .AndFilter(blog => blog.Id == id);

            var blogResult = await repository.SingleOrDefaultAsync(query)
                                             .ConfigureAwait(continueOnCapturedContext: false);

            return Ok(blogResult);
        }

        // POST: api/Blogs
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Blog model)
        {
            var repository = _unitOfWork.Repository<Blog>();

            await repository.InsertOneAsync(model)
                            .ConfigureAwait(continueOnCapturedContext: false);

            await _unitOfWork.SaveChangesAsync()
                             .ConfigureAwait(continueOnCapturedContext: false);

            return NoContent();
        }

        // PUT: api/Blogs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Blog model)
        {
            var repository = _unitOfWork.Repository<Blog>();

            if (!await repository.AnyAsync(blog => blog.Id == id)
                                 .ConfigureAwait(continueOnCapturedContext: false))
            {
                return Conflict();
            }

            // Example: Update Properties
            repository.UpdateOne(
                blog => blog.Id == id,
                model,
                new Expression<Func<Blog, object>>[] { x => x.Title });

            // Example: Update Model
            repository.ReplaceOne(blog => blog.Id == id, model);

            await _unitOfWork.SaveChangesAsync()
                             .ConfigureAwait(continueOnCapturedContext: false);

            return NoContent();
        }

        // PUT: api/Blogs/5
        [HttpPut("{id}/Title", Name = "UpdateTitle")]
        public async Task<IActionResult> Put(int id, [FromBody] string title)
        {
            var repository = _unitOfWork.Repository<Blog>();

            if (!await repository.AnyAsync(blog => blog.Id == id)
                                 .ConfigureAwait(continueOnCapturedContext: false))
            {
                return Conflict();
            }

            // Example: IClientSessionHandle
            _unitOfWork.StartTransaction();

            // Without Parameters
            await repository.UpdateOneAsync(
                                blog => blog.Id == id,
                                new Blog { Id = id, Title = title },
                                new Expression<Func<Blog, object>>[] { x => x.Title })
                            .ConfigureAwait(continueOnCapturedContext: false);

            await _unitOfWork.SaveChangesAsync()
                             .ConfigureAwait(continueOnCapturedContext: false);

            await _unitOfWork.CommitTransactionAsync()
                             .ConfigureAwait(continueOnCapturedContext: false);

            return NoContent();
        }

        // DELETE: api/Blogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var repository = _unitOfWork.Repository<Blog>();

            if (!await repository.AnyAsync(blog => blog.Id == id)
                                 .ConfigureAwait(continueOnCapturedContext: false))
            {
                return Conflict();
            }

            await repository.DeleteOneAsync(x => x.Id == id)
                            .ConfigureAwait(continueOnCapturedContext: true);

            await _unitOfWork.SaveChangesAsync()
                             .ConfigureAwait(continueOnCapturedContext: false);

            return NoContent();
        }

        #region IDisposable Members

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable Members
    }
}
