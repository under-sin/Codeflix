namespace FC.Codeflix.Catalog.Application.Interface;

public interface IUnitOfWork
{
    public Task Commit(CancellationToken cancellationToken);
}