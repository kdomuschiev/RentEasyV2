namespace renteasy_api.Common;

public static class Constants
{
    public const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB
    public const int SignedUrlInlineExpiryHours = 1;
    public const int SignedUrlDownloadExpiryHours = 24;
    public const int NudgeJobPollingIntervalMinutes = 5;
    public const int MaxConditionReportDisputeRounds = 3;
    public const int TenancyExpiryMonths = 12;
    public const int JwtExpiryDays = 7;
    public const int PasswordResetTokenExpiryHours = 1;
}
