namespace ManualDtoMappingDemo.Endpoints;

public class ProductEndpoints
{
	public void MapProductEndpoints (IEndpointRouteBuilder app)
	{
		var productGroup = app.MapGroup ("/products");

		productGroup.MapGet ("/", async (
			IProductRepository productRepository,
			CancellationToken cancellationToken) =>
		{
			var products = (await productRepository.GetAllAsync (cancellationToken))
				.Select (p => p.ToDto ());

			return TypedResults.Ok (products);
		});

		productGroup.MapGet ("/{id:Guid}", async (
			[FromRoute] Guid id,
			IProductRepository productRepository,
			CancellationToken cancellationToken) =>
		{
			var product = await productRepository.GetByIdAsync (id, cancellationToken);

			return product is null
				? Results.NotFound ()
				: TypedResults.Ok (product.ToDto ());
		}).WithName ("GetProductById");

		productGroup.MapPost ("/", async (
				[FromBody] CreateProductRequest request,
				IProductRepository productRepository,
				HttpContext httpContext,
				CancellationToken cancellationToken) =>
			{
				var product = request.ToEntity ();

				await productRepository.AddAsync (product, cancellationToken);

				var uri = httpContext
					.RequestServices
					.GetRequiredService<LinkGenerator> ()
					.GetUriByName (httpContext, "GetProductById", new { product.Id });

				return TypedResults.Created (uri, product.Id);
			});

		productGroup.MapPut ("/", async (
			[FromBody] UpdateProductRequest request,
			IProductRepository productRepository,
			CancellationToken cancellationToken) =>
		{
			if (request.Id == Guid.Empty)
				return Results.BadRequest ("Product Id is required");

			var existingProduct = await productRepository.GetByIdAsync (request.Id, cancellationToken);

			if (existingProduct is null)
				return Results.NotFound ();

			await productRepository.UpdateAsync (request.ToEntity (existingProduct), cancellationToken);

			return TypedResults.Ok ("Update Successful");
		});

		productGroup.MapDelete ("/{id:Guid}", async (
			[FromRoute] Guid id,
			IProductRepository productRepository,
			CancellationToken cancellationToken) =>
		{
			var product = await productRepository.GetByIdAsync (id, cancellationToken);

			if (product is null)
				return Results.NotFound ();

			await productRepository.DeleteAsync (product, cancellationToken);

			return TypedResults.NoContent ();
		});
	}
}