namespace backend.DTOs;

public record ProductDto(
    int Id,
    string Name,
    decimal Price,
    int Stock,
    int CategoryId,
    string Category
);

public record CreateProductDto(
    string Name,
    decimal Price,
    int Stock,
    int CategoryId
);

public record UpdateProductDto(
    string Name,
    decimal Price,
    int Stock,
    int CategoryId
);