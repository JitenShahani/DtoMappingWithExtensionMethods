namespace ManualDtoMappingDemo.Repositories;

public class ProductRepository : IProductRepository
{
	private readonly ProductDbContext _dbContext;

	public ProductRepository(ProductDbContext dbContext)
		=> _dbContext = dbContext;

	public async Task<IEnumerable<Product>> GetAllAsync (CancellationToken cancellationToken)
	{
		return await _dbContext.Products.AsNoTracking().ToListAsync (cancellationToken);
	}

	public async Task<Product?> GetByIdAsync (Guid id, CancellationToken cancellationToken)
	{
		return await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync (p => p.Id == id, cancellationToken); 
	}

	public async Task AddAsync (Product product, CancellationToken cancellationToken)
	{
		_dbContext.Products.Add (product);
		await _dbContext.SaveChangesAsync (cancellationToken);
	}

	public async Task UpdateAsync (Product product, CancellationToken cancellationToken)
	{
		_dbContext.Products.Update (product);
		await _dbContext.SaveChangesAsync (cancellationToken);
	}

	public async Task DeleteAsync (Product product, CancellationToken cancellationToken)
	{
		_dbContext.Products.Remove (product);
		await _dbContext.SaveChangesAsync (cancellationToken);
	}
}