namespace ManualDtoMappingDemo.Mapping;

public static class ProductDtoExtensions
{
	public static ProductResponse ToDto (this Product product)
		=> new (product.Id, product.Name, product.Description, product.Quantity, product.Price);

	public static Product ToEntity (this CreateProductRequest request)
		=> new ()
		{
			Name = request.Name,
			Description = request.Description,
			Quantity = request.Quantity,
			Price = request.Price,
			CreatedAt = DateTime.Now
		};

	public static Product ToEntity (this UpdateProductRequest request, Product existingProduct)
		=> new()
		{
			Id = request.Id,
			Name = request.Name,
			Description = request.Description,
			Quantity = request.Quantity,
			Price = request.Price,

			// Since, PUT updates the entire record, we need to preserve the CreatedAt column value.
			// Otherwise, it will be overwritten with a blank value since it is not part of the UpdateProductRequest.
			// This is a common pattern to ensure audit fields remain intact during updates.
			CreatedAt = existingProduct.CreatedAt
		};
}