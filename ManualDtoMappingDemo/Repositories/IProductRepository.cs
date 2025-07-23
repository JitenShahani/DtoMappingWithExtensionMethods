namespace ManualDtoMappingDemo.Repositories;

public interface IProductRepository
{
	Task<IEnumerable<Product>> GetAllAsync (CancellationToken cancellationToken);
	Task<Product?> GetByIdAsync (Guid id, CancellationToken cancellationToken);
	Task AddAsync (Product product, CancellationToken cancellationToken);
	Task UpdateAsync (Product product, CancellationToken cancellationToken);
	Task DeleteAsync (Product product, CancellationToken cancellationToken);
}