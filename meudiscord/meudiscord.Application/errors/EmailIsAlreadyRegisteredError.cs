public record EmailIsAlreadyRegisteredError() : AppError("Email ja cadastrado", ErrorType.Validation.ToString(), nameof(EmailIsAlreadyRegisteredError));