using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MoneyLoris.Application.Common.Base;
using MoneyLoris.Application.Common.Interfaces;
using MoneyLoris.Application.Shared;
using MoneyLoris.Infrastructure.Persistence.Context;

namespace MoneyLoris.Infrastructure.Persistence.Repositories.Base;
public class RepositoryBase<T> : IRepositoryBase<T> where T : EntityBase
{
    private readonly BaseApplicationDbContext _context;
    protected readonly DbSet<T> _dbset;
    private IDbContextTransaction _transaction = null!;

    public RepositoryBase(BaseApplicationDbContext context)
    {
        _context = context;
        _dbset = context.Set<T>();
    }


    public async Task BeginTransaction()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransaction()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
        }
        _transaction = null!;
    }

    public async Task RollbackTransaction()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
        }
        _transaction = null!;
    }


    public async Task<T> GetById(int id)
    {
        var entity = await _dbset.FindAsync(id);
        return entity;
    }

    public async Task<T> Insert(T? model)
    {
        if (model is null)
        {
            return model!;
        }

        await _dbset.AddAsync(model);
        await _context.SaveChangesAsync();

        return model;
    }

    public async Task<bool> Update(T? model)
    {
        T? entity;

        if (model is null || (entity = await _dbset.FirstOrDefaultAsync(s => s.Id == model.Id)) is null)
        {
            return false;
        }

        _context.Entry(entity).CurrentValues.SetValues(model);
        _dbset.Update(entity!);

        return await _context.SaveChangesAsync() > 0;
    }

    //TODO - remover
    public async Task<bool> Delete(T? model)
    {
        if (model is null)
        {
            return false;
        }

        _dbset.Remove(model);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _dbset.FindAsync(id);

        if (entity is null)
        {
            return false;
        }

        _dbset.Remove(entity);

        return await _context.SaveChangesAsync() > 0;
    }

    //TODO - remover
    public async Task<T> Get(int id, Expression<Func<T, object>>? includes = null)
    {
        T? model = null;

        if (includes is not null)
        {
            var query = _dbset.IncludeT(includes);

            model = await query.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        model ??= await _dbset.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        return model!;
    }

    //TODO - remover
    public virtual async Task<T> Get(
        Expression<Func<T, bool>> where,
        Expression<Func<T, object>>? includes = null
    )
    {
        T? model = null;

        if (includes is not null)
        {
            var query = _dbset.IncludeT(includes);

            model = await query.AsNoTracking()
                .Where(where)
                .FirstOrDefaultAsync();
        }

        model ??= await _dbset.AsNoTracking()
            .Where(where)
            .FirstOrDefaultAsync();

        return model!;
    }

    //TODO - remover
    public async Task<IEnumerable<T>> GetAll(
        Expression<Func<T, bool>>? where = null,
        Expression<Func<T, object>>? includes = null
    )
    {
        List<T>? list = null;

        if (includes is not null)
        {
            var query = _dbset.IncludeT(includes);
            list = await query.AsNoTracking()
                .ToListAsync();
        }

        if (where is not null)
        {
            list = await _dbset.AsNoTracking()
                .Where(where)
                .ToListAsync();
        }

        list ??= await _dbset.AsNoTracking()
            .ToListAsync();

        return list;
    }

    //TODO - remover
    public async Task<ICollection<T>> GetPaged(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? where = null,
        Expression<Func<T, object>>? orderBy = null
    )
    {
        var startingRecordNumber = (pageSize * (page - 1));
        List<T>? list = null;

        if (where is not null)
        {
            if (orderBy is not null)
            {
                list = await _dbset.AsNoTracking()
                    .Where(where)
                    .OrderBy(orderBy)
                    .Skip(startingRecordNumber)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                list = await _dbset.AsNoTracking()
                    .Where(where)
                    .Skip(startingRecordNumber)
                    .Take(pageSize)
                    .ToListAsync();
            }
        }

        list ??= await _dbset.AsNoTracking()
            .Skip(startingRecordNumber)
            .Take(pageSize)
            .ToListAsync();

        return list;
    }

    //TODO - remover
    public async Task<int> GetTotal(Expression<Func<T, bool>>? where = null)
    {
        if (where is null)
        {
            return await _dbset.AsNoTracking().CountAsync();
        }

        return await _dbset.AsNoTracking().CountAsync(where);
    }
}


internal static class RepositoryExtensions
{
    public static IQueryable<T> IncludeT<T>(
        this DbSet<T> dbSet,
        params Expression<Func<T, object>>[] includes
    )
        where T : class
    {
        IQueryable<T>? query = null;

        foreach (var include in includes)
        {
            query = dbSet.Include(include);
        }

        return query ?? dbSet;
    }

    public static IQueryable<T> IncluiPaginacao<T>(this IQueryable<T> query, int page, int pageSize)
    {
        var startingRecordNumber = (pageSize * (page - 1));

        var q = query.Skip(startingRecordNumber).Take(pageSize);

        return q;
    }

    public static IQueryable<T> IncluiPaginacao<T>(this IQueryable<T> query, PaginationFilter filtro)
    {
        var startingRecordNumber = (filtro.ResultsPerPage * (filtro.CurrentPage - 1));

        var q = query.Skip(startingRecordNumber).Take(filtro.ResultsPerPage);

        return q;
    }
}