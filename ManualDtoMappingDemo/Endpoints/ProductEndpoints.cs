namespace ManualDtoMappingDemo.Endpoints;

public class ProductEndpoints
{
	public void MapProductEndpoints (IEndpointRouteBuilder app)
	{
		var productGroup = app.MapGroup ("/products");

		productGroup.MapGet ("/", async (IProductRepository productRepository, CancellationToken cancellationToken) =>
		{
			var products = (await productRepository.GetAllAsync (cancellationToken))
				.Select (p => p.ToDto());

			return TypedResults.Ok (products);
		});

		productGroup.MapGet ("/{id:Guid}", async ([FromRoute] Guid id, IProductRepository productRepository, CancellationToken cancellationToken) =>
		{
			var product = await productRepository.GetByIdAsync (id, cancellationToken);
			
			return product is null
				? Results.NotFound()
				: TypedResults.Ok (product.ToDto());
		}).WithName ("GetProductById");

		productGroup.MapPost ("/",
			async ([FromBody] CreateProductRequest request, IProductRepository productRepository, HttpContext httpContext,
				CancellationToken cancellationToken) =>
			{
				// Product product = new()
				// {
				// 	Id = Guid.CreateVersion7(),
				// 	Name = request.Name,
				// 	Description = request.Description,
				// 	Quantity = request.Quantity,
				// 	Price = request.Price
				// };

				var product = request.ToEntity();

				await productRepository.AddAsync (product, cancellationToken);
				
				var uri = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}" +
				          httpContext.RequestServices.GetRequiredService<LinkGenerator>().GetPathByName ("GetProductById", new { product.Id });

				return TypedResults.Created (uri, product.Id);
			});

		productGroup.MapPut ("/", async ([FromBody] UpdateProductRequest request, IProductRepository productRepository, CancellationToken cancellationToken) =>
		{
			if (request.Id == Guid.Empty)
				return Results.BadRequest ("Product Id is required");

			var product = await productRepository.GetByIdAsync (request.Id, cancellationToken);

			if (product is null)
				return Results.NotFound();

			// product.Name = request.Name;
			// product.Description = request.Description;
			// product.Quantity = request.Quantity;
			// product.Price = request.Price;

			await productRepository.UpdateAsync (request.ToEntity(), cancellationToken);

			return TypedResults.Ok ("Update Successful");
		});

		productGroup.MapDelete ("/{id:Guid}", async (Guid id, IProductRepository productRepository, CancellationToken cancellationToken) =>
		{
			var product = await productRepository.GetByIdAsync (id, cancellationToken);

			if (product is null)
				return Results.NotFound();

			await productRepository.DeleteAsync (product, cancellationToken);

			return TypedResults.NoContent();
		});
	}
}