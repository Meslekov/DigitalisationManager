namespace DigitalisationManager.GCommon
{
    public static class ValidationConstants
    {
        public static class Fund
        {
            public const int CodeMin = 2;
            public const int CodeMax = 50;

            public const int TitleMin = 3;
            public const int TitleMax = 200;

            public const int DescriptionMax = 2000;
        }

        public static class Item
        {
            public const int InventoryMin = 1;
            public const int InventoryMax = 50;

            public const int TitleMin = 3;
            public const int TitleMax = 200;

            public const int DescriptionMax = 2000;
            public const int DocumentDateMax = 200;
        }

        public static class DigitalFile
        {
            public const int OriginalNameMax = 255;
            public const int StoredNameMax = 80;
            public const int PathMax = 500;
            public const int ChecksumMax = 64;
            public const int ContentTypeMax = 100;
        }

        public static class Category
        {
            public const int NameMin = 2;
            public const int NameMax = 100;

            public const int DescriptionMax = 500;
        }

        public static class ItemHistory
        {
            public const int ActionMin = 3;
            public const int ActionMax = 100;

            public const int DescriptionMax = 1000;
        }


    }
}
