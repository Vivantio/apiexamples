namespace VivantioApiInteractive.Utility;

internal enum InfoType
{
    Unknown,
    Permission
}

internal enum ErrorType
{
    Unknown,
    FieldMissing,
    FieldInvalid,
    ItemNotFound,
    ItemNotValidForAction,
    ItemPermissionDeniedForAction
}
internal enum SystemAreaId
{
    Client = 0,
    Location = 1,
    Caller = 2,
    Ticket = 3,
    Asset = 4,
    Article = 5,
}
internal enum AttachmentFileType
{
    PDF,
    Text
}

internal enum AttachmentType
{
    File = 0,
    Url = 1,
}

internal enum QueryMode
{
    MatchAll,
    MatchAny,
    MatchNone
}

internal enum Operator
{
    Equals,
    DoesNotEqual
}

internal enum StatusType
{
    Open = 1,
    OnHold = 2,
    Closed = 4,
    Pending = 5,
    Deleted = 99
}
