using System.Linq.Expressions;
using MoneyLoris.Application.Common.Base;

namespace MoneyLoris.Application.Common.Interfaces;
public interface IRepositoryBase<T> where T : EntityBase
{
    Task BeginTransaction();
    Task CommitTransaction();
    Task RollbackTransaction();

    Task<T> GetById(int id);
    Task<T> Insert(T? model);
    Task<bool> Update(T? model);
    Task<bool> Delete(int id);

    //TODO - apagar os comentados

    //Task<bool> Delete(T? model);


    //Task<T> Get(int id, Expression<Func<T, object>>? includes = null);

    //Task<T> Get(Expression<Func<T, bool>> where, Expression<Func<T, object>>? includes = null);

    //Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? where = null, Expression<Func<T, object>>? includes = null);

    //Task<ICollection<T>> GetPaged(int page, int pageSize, Expression<Func<T, bool>>? where = null, Expression<Func<T, object>>? orderBy = null);

    //Task<int> GetTotal(Expression<Func<T, bool>>? where = null);
}
