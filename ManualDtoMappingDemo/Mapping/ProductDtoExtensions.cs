namespace ManualDtoMappingDemo.Mapping;

public static class ProductDtoExtensions
{
	public static ProductResponse ToDto (this Product product)
		=> new(product.Id, product.Name, product.Description, product.Quantity, product.Price);

	public static Product ToEntity (this CreateProductRequest request)
		=> new()
		{
			Name = request.Name,
			Description = request.Description,
			Quantity = request.Quantity,
			Price = request.Price,
			CreatedAt = DateTime.Now
		};

	public static Product ToEntity (this UpdateProductRequest request)
		=> new()
		{
			Id = request.Id,
			Name = request.Name,
			Description = request.Description,
			Quantity = request.Quantity,
			Price = request.Price
		};
}