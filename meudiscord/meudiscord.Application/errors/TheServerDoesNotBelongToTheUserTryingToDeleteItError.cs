public record TheServerDoesNotBelongToTheUserTryingToDeleItError():AppError("O servidor não pertence ao usuário que esta tentando deleta-lo", ErrorType.Business, nameof(TheServerDoesNotBelongToTheUserTryingToDeleItError));