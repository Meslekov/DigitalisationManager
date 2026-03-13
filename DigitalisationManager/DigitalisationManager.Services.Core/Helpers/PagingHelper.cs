namespace DigitalisationManager.Services.Core.Helpers
{
    using static DigitalisationManager.GCommon.ApplicationConstants.PagingConstants;

    public static class PagingHelper
    {
        public static int NormalizePage(int page)
        {
            if (page < 1)
            {
                return DefaultPage;
            }

            return page;
        }

        public static int NormalizePageSize(int pageSize)
        {
            if (pageSize < MinPageSize)
            {
                return DefaultPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                return MaxPageSize;
            }

            return pageSize;
        }
    }
}