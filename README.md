# MongoDB.DataAccess

It's a modern and generic data access structure for .NET and MongoDB. It supports UnitOfWork, Repository and QueryBuilder patterns. It also includes DbContext, IdGenerators and transactions support with replica set.

## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

## Status

[![build-and-publish Workflow Status](https://github.com/ffernandolima/mongo-db-data-access/actions/workflows/build-and-publish.yml/badge.svg?branch=main)](https://github.com/ffernandolima/mongo-db-data-access/actions/workflows/build-and-publish.yml/branch=main)

 | Package | NuGet |
 | ------- | ------- |
 | MongoDB.Data.Infrastructure.Abstractions | [![Nuget](https://img.shields.io/badge/nuget-v1.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/MongoDB.Data.Infrastructure.Abstractions)](https://www.nuget.org/packages/MongoDB.Data.Infrastructure.Abstractions/1.2.1) |
 | MongoDB.Data.QueryBuilder.Abstractions | [![Nuget](https://img.shields.io/badge/nuget-v1.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/MongoDB.Data.QueryBuilder.Abstractions)](https://www.nuget.org/packages/MongoDB.Data.QueryBuilder.Abstractions/1.2.1) |
 | MongoDB.Data.Repository.Abstractions | [![Nuget](https://img.shields.io/badge/nuget-v1.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/MongoDB.Data.Repository.Abstractions)](https://www.nuget.org/packages/MongoDB.Data.Repository.Abstractions/1.2.1) |
 | MongoDB.Data.UnitOfWork.Abstractions | [![Nuget](https://img.shields.io/badge/nuget-v1.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/MongoDB.Data.UnitOfWork.Abstractions)](https://www.nuget.org/packages/MongoDB.Data.UnitOfWork.Abstractions/1.2.1) |
 | ------- | ------- |
 | MongoDB.Data.Generators | [![Nuget](https://img.shields.io/badge/nuget-v1.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/MongoDB.Data.Generators)](https://www.nuget.org/packages/MongoDB.Data.Generators/1.2.1) |
 | MongoDB.Data.Infrastructure | [![Nuget](https://img.shields.io/badge/nuget-v1.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/MongoDB.Data.Infrastructure)](https://www.nuget.org/packages/MongoDB.Data.Infrastructure/1.2.1) |
 | MongoDB.Data.QueryBuilder | [![Nuget](https://img.shields.io/badge/nuget-v1.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/MongoDB.Data.QueryBuilder)](https://www.nuget.org/packages/MongoDB.Data.QueryBuilder/1.2.1) |
 | MongoDB.Data.Repository | [![Nuget](https://img.shields.io/badge/nuget-v1.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/MongoDB.Data.Repository)](https://www.nuget.org/packages/MongoDB.Data.Repository/1.2.1) |
 | MongoDB.Data.UnitOfWork | [![Nuget](https://img.shields.io/badge/nuget-v1.2.1-blue) ![Nuget](https://img.shields.io/nuget/dt/MongoDB.Data.UnitOfWork)](https://www.nuget.org/packages/MongoDB.Data.UnitOfWork/1.2.1) |

## Installation

MongoDB.DataAccess is available on Nuget.

```
Install-Package MongoDB.Data.Infrastructure.Abstractions -Version 1.2.1
Install-Package MongoDB.Data.QueryBuilder.Abstractions -Version 1.2.1
Install-Package MongoDB.Data.Repository.Abstractions -Version 1.2.1
Install-Package MongoDB.Data.UnitOfWork.Abstractions -Version 1.2.1

Install-Package MongoDB.Data.Generators -Version 1.2.1
Install-Package MongoDB.Data.Infrastructure -Version 1.2.1
Install-Package MongoDB.Data.QueryBuilder -Version 1.2.1
Install-Package MongoDB.Data.Repository -Version 1.2.1
Install-Package MongoDB.Data.UnitOfWork -Version 1.2.1
```

**P.S.: MongoDB.Data.UnitOfWork depends on the other packages, so installing this package is enough.**

## Usage

#### The following code demonstrates basic usage of UnitOfWork, Repository and QueryBuilder patterns.

First of all, please register the dependencies into the MS Built-In container:

```C#
public class BloggingContext : MongoDbContext
{
    public BloggingContext(IMongoClient client, IMongoDatabase database, IMongoDbContextOptions options)
        : base(client, database, options)
    { }
}

// Register the DbContext
services.AddMongoDbContext<IMongoDbContext, BloggingContext>(
    connectionString: Configuration.GetValue<string>("MongoSettings:ConnectionString"),
    databaseName: Configuration.GetValue<string>("MongoSettings:DatabaseName"),
    configureFluentConfigurationOptions: options => options.ScanningAssemblies = new[] { typeof(BloggingContext).Assembly });

// Register the UnitOfWork
services.AddMongoDbUnitOfWork<BloggingContext>();
```

After that, use the structure in your code like that:

```C#
private readonly IMongoDbUnitOfWork<BloggingContext> _unitOfWork;
	
// Injection
public BlogsController(IMongoDbUnitOfWork<BloggingContext> unitOfWork) 
    => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork), $"{nameof(unitOfWork)} cannot be null.");

public void GetAllBlogs()
{
    var repository = _unitOfWork.Repository<Blog>();

    var query = repository.MultipleResultQuery();

    var blogs = repository.Search(query);
}

public void GetAllBlogsProjection()
{
    var repository = _unitOfWork.Repository<Blog>();

    var query = repository.MultipleResultQuery()
                          .Select(selector => new 
                          {
                              Name = selector.Title, 
                              Link = selector.Url, 
                              Type = selector.Type.Description 
                          });

    var blogs = repository.Search(query);
}

public void GetAllOrderedBlogs()
{
    var repository = _unitOfWork.Repository<Blog>();

    IMongoDbQuery<Blog> query;
    IList<Blog> blogs;

    query = repository.MultipleResultQuery()
                      .OrderByDescending("Id");

    blogs = repository.Search(query);

    query = repository.MultipleResultQuery()
                      .OrderByDescending(blog => blog.Id);

    blogs = repository.Search(query);
}

public void GetTopBlogs()
{
    var repository = _unitOfWork.Repository<Blog>();

    var query = repository.MultipleResultQuery()
                          .Top(10);

    var blogs = repository.Search(query);
}

public void GetPagedBlogs()
{
    var repository = _unitOfWork.Repository<Blog>();

    var query = repository.MultipleResultQuery()
                          .Page(1, 20);

    var blogs = repository.Search(query);
}

public void GetBlogsPagedList()
{
    var repository = _unitOfWork.Repository<Blog>();

    var query = repository.MultipleResultQuery()
                          .Page(1, 20);

    var blogs = repository.Search(query)
                          .ToPagedList(
                            query.Paging.PageIndex,
                            query.Paging.PageSize,
                            query.Paging.TotalCount);
}

public void GetFilteredBlogs()
{
    var repository = _unitOfWork.Repository<Blog>();

    var query = repository.MultipleResultQuery()
                          .AndFilter(blog => blog.Url.StartsWith("/a/"))
                          .AndFilter(blog => blog.Title.StartsWith("a"))
                          .AndFilter(blog => blog.Posts.Any());

    var blogs = repository.Search(query);
}

public void GetUrls()
{
    var repository = _unitOfWork.CustomRepository<ICustomBlogRepository>();

    var urls = repository.GetAllBlogUrls();
}

public void GetBlogByUrl()
{
    var repository = _unitOfWork.Repository<Blog>();

    var query = repository.SingleResultQuery()
                          .AndFilter(blog => blog.Url.StartsWith("/a/"))
                          .OrderByDescending(blog => blog.Id);

    var blogResult = repository.FirstOrDefault(query);
}

public void GetBlogById()
{
    var repository = _unitOfWork.Repository<Blog>();

    var query = repository.SingleResultQuery()
                          .AndFilter(blog => blog.Id == 1);

    var blogResult = repository.SingleOrDefault(query);
}

public void GetBlogByIdProjection()
{
    var repository = _unitOfWork.Repository<Blog>();

    var query = repository.SingleResultQuery()
                          .AndFilter(blog => blog.Id == 1)
                          .Select(selector => new 
                          {
                              selector.Id, 
                              Name = selector.Title, 
                              Link = selector.Url, 
                              Type = selector.Type.Description
                          });

    var blogResult = repository.SingleOrDefault(query);
}

public void ExistsBlog()
{
    var repository = _unitOfWork.Repository<Blog>();

    var exists = repository.Any(blog => blog.Url.StartsWith("/a/"));
}

public void GetBlogCount()
{
    var repository = _unitOfWork.Repository<Blog>();

    var count = repository.Count();

    var longCount = repository.LongCount();
}

public void MaxBlogId()
{
    var repository = _unitOfWork.Repository<Blog>();

    var id = repository.Max(blog => blog.Id);
}

public void MinBlogId()
{
    var repository = _unitOfWork.Repository<Blog>();

    var id = repository.Min(blog => blog.Id);
}

public void AddBlog()
{
    var repository = _unitOfWork.Repository<Blog>();

    repository.InsertOne(Seeder.SeedBlog(51));

    _unitOfWork.SaveChanges();
}

public void UpdateBlog()
{
    var repository = _unitOfWork.Repository<Blog>();

    repository.ReplaceOne(x => x.Id == id, model);

    _unitOfWork.SaveChanges();
}

public void DeleteBlog()
{
    var repository = _unitOfWork.Repository<Blog>();

    repository.DeleteOne(x => x.Id == id);

    _unitOfWork.SaveChanges();
}
```

The operations above are also available as async.

Please check some available samples [here](https://github.com/ffernandolima/mongo-db-data-access/tree/main/samples)

## Support / Contributing
If you want to help with the project, feel free to open pull requests and submit issues. 

## Donate

If you would like to show your support for this project, then please feel free to buy me a coffee.

<a href="https://www.buymeacoffee.com/fernandolima" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/white_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>
