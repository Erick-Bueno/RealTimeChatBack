public record UnableToDeleteServerError() : AppError("Não foi possivel deletar o servidor",ErrorType.Business,nameof(UnableToDeleteServerError));