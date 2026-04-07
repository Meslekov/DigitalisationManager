namespace DigitalisationManager.GCommon
{
    public static class ApplicationConstants
    {
        public static class PagingConstants
        {
            public const int DefaultPage = 1;
            public const int DefaultPageSize = 20;
            public const int MinPageSize = 1;
            public const int MaxPageSize = 100;
            public const int DefaultFilesPageSize = 20;
        }

        public static class RoleNames
        {
            public const string User = "User";
            public const string Manager = "Manager";
            public const string Administrator = "Administrator";
        }
    }
}