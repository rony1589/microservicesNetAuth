using FluentValidation;
using ProductsService.Application.Products.CQRS;

namespace ProductsService.Application.Products.Validators;

// Validador base genérico para comandos que implementen IProductWrite
public class ProductWriteValidator<T> : AbstractValidator<T> where T : IProductWrite
{
    public ProductWriteValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(60);
    }
}

public class CreateProductValidator : ProductWriteValidator<CreateProductCommand> { }

public class UpdateProductValidator : ProductWriteValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
