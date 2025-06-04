namespace VivantioApiInteractive
{
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
}
