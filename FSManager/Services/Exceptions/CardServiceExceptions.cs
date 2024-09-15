using System.ComponentModel.DataAnnotations;

namespace FSManager.Services.Exceptions;

[System.Serializable]
public class CardServiceException : System.Exception
{
    public CardServiceException() { }
    public CardServiceException(string message) : base(message) { }
    public CardServiceException(string message, System.Exception inner) : base(message, inner) { }
}

[System.Serializable]
public class CardNotFoundException : CardServiceException
{
    public CardNotFoundException() { }
    public CardNotFoundException(string key) : base($"Card with key {key} not found") { }
    public CardNotFoundException(string message, System.Exception inner) : base(message, inner) { }
}

[System.Serializable]
public class FailedToDeleteCardException : CardServiceException
{
    public FailedToDeleteCardException() { }
    public FailedToDeleteCardException(string message) : base(message) { }
    public FailedToDeleteCardException(string message, System.Exception inner) : base(message, inner) { }
}

[System.Serializable]
public class RelationAlreadyExistsException : CardServiceException
{
    public RelationAlreadyExistsException() { }
    public RelationAlreadyExistsException(string cardKey, string relatedCardKey) : base($"Relation between {cardKey} and {relatedCardKey} already exists") { }
    public RelationAlreadyExistsException(string message, System.Exception inner) : base(message, inner) { }
}

[System.Serializable]
public class RelationWithSelfException : CardServiceException
{
    public RelationWithSelfException() { }
    public RelationWithSelfException(string message) : base(message) { }
    public RelationWithSelfException(string message, System.Exception inner) : base(message, inner) { }
}

[System.Serializable]
public class RelationNotFoundException : CardServiceException
{
    public RelationNotFoundException() { }
    public RelationNotFoundException(string cardKey, string relatedCardKey) : base($"Relation between {cardKey} and {relatedCardKey} not found") { }
    public RelationNotFoundException(string message, System.Exception inner) : base(message, inner) { }
}

[System.Serializable]
public class CardKeyAlreadyExistsException : CardServiceException
{
    public CardKeyAlreadyExistsException() { }
    public CardKeyAlreadyExistsException(string message) : base(message) { }
    public CardKeyAlreadyExistsException(string message, System.Exception inner) : base(message, inner) { }
}


[System.Serializable]
public class CardValidationException : CardServiceException
{
    public CardValidationException() { }
    public CardValidationException(string message) : base(message) { }
    public CardValidationException(string prefix, List<ValidationResult> validationResults) 
        : base(
            $"{prefix}\n\t" + string.Join("\n\t", validationResults.Select(r => r.ErrorMessage))
        )
    { }
    public CardValidationException(string message, System.Exception inner) : base(message, inner) { }
}
