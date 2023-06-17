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
}
