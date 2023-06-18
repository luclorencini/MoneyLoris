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
    protected readonly BaseApplicationDbContext _context;
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
}


internal static class RepositoryExtensions
{
    public static IQueryable<T> IncluiPaginacao<T>(this IQueryable<T> query, PaginationFilter filtro)
    {
        var startingRecordNumber = (filtro.ResultsPerPage * (filtro.CurrentPage - 1));

        var q = query.Skip(startingRecordNumber).Take(filtro.ResultsPerPage);

        return q;
    }
}