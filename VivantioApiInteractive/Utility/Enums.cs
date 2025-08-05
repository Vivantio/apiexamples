namespace VivantioApiInteractive.Utility;

public enum InfoType
{
    Unknown,
    Permission
}

public enum ErrorType
{
    Unknown,
    FieldMissing,
    FieldInvalid,
    ItemNotFound,
    ItemNotValidForAction,
    ItemPermissionDeniedForAction
}
public enum SystemAreaId
{
    Client = 0,
    Location = 1,
    Caller = 2,
    Ticket = 3,
    Asset = 4,
    Article = 5,
}
public enum AttachmentFileType
{
    PDF,
    Text
}

public enum QueryMode
{
    MatchAll,
    MatchAny,
    MatchNone
}

public enum Operator
{
    Equals,
    DoesNotEqual
}

public enum StatusType
{
    Open = 1,
    OnHold = 2,
    Closed = 4,
    Pending = 5,
    Deleted = 99
}
