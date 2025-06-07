using Entity.Dtos;
using FluentValidation;

namespace Business.Validators
{
    public class ClienteCreateDtoValidator : AbstractValidator<ClienteCreateDto>
    {
        public ClienteCreateDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El nombre solo puede contener letras y espacios");

            RuleFor(x => x.Apellido)
                .NotEmpty().WithMessage("El apellido es obligatorio")
                .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres")
                .Matches(@"^[a-zA-Záéíóú