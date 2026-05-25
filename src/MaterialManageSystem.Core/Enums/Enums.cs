namespace MaterialManageSystem.Core.Enums;

public enum ReelStatus : int
{
    InStock = 0,
    OutStock = 1,
    Online = 2,
    Scrapped = 3
}

public enum UsageType : int
{
    Checkout = 0,
    OnlineUse = 1,
    Return = 2,
    Scrap = 3
}

public enum StorageMode : int
{
    SamePartNoMultipleReels = 0,
    SingleReelExclusive = 1,
    NoRestriction = 2
}

public enum WarningType : int
{
    Quantity = 0,
    Time = 1,
    Both = 2
}

public enum WarningLevel : int
{
    Normal = 0,
    Notice = 1,
    Warning = 2,
    Severe = 3
}

public enum UserType : int
{
    Normal = 0,
    Admin = 1,
    SystemAdmin = 2
}
