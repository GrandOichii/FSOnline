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
public class RelationWithSelfException : System.Exception
{
    public RelationWithSelfException() { }
    public RelationWithSelfException(string message) : base(message) { }
    public RelationWithSelfException(string message, System.Exception inner) : base(message, inner) { }
}

[System.Serializable]
public class RelationNotFoundException : System.Exception
{
    public RelationNotFoundException() { }
    public RelationNotFoundException(string cardKey, string relatedCardKey) : base($"Relation between {cardKey} and {relatedCardKey} not found") { }
    public RelationNotFoundException(string message, System.Exception inner) : base(message, inner) { }
}