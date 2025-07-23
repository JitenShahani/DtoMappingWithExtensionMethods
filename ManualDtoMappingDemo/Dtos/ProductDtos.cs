namespace ManualDtoMappingDemo.Dtos;

public sealed record CreateProductRequest (string Name, string Description, int Quantity, decimal Price);

public sealed record UpdateProductRequest (Guid Id, string Name, string Description, int Quantity, decimal Price);

public sealed record ProductResponse (Guid Id, string Name, string Description, int Quantity, decimal Price);