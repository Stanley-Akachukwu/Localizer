//using UpDash.Shared.Domain.Enums.Documents;
//using UpDash.Shared.Domain.Enums.Securities;
//public static class EnumKeyValueHelper
//{
//    public static IEnumerable<object> GetMimeTypeKeyValues()
//    {
//        return Enum.GetValues(typeof(MimeTypes))
//            .Cast<MimeTypes>()
//            .Select(e => new { Key = (int)e, Value = e.ToString() });
//    }

//    public static IEnumerable<object> GetFileTypeKeyValues()
//    {
//        return Enum.GetValues(typeof(FileType))
//            .Cast<FileType>()
//            .Select(e => new { Key = (int)e, Value = e.ToString() });
//    }

//    public static UpDashUserType ParseUserType(string value)
//    {
//        if (string.IsNullOrWhiteSpace(value))
//            return UpDashUserType.None;

//        // Try to parse as number first
//        if (int.TryParse(value, out var numericValue))
//        {
//            if (Enum.IsDefined(typeof(UpDashUserType), numericValue))
//                return (UpDashUserType)numericValue;
//            else
//                return UpDashUserType.None;
//        }

//        // Try to parse as name (case-insensitive)
//        if (Enum.TryParse<UpDashUserType>(value, true, out var parsedEnum))
//        {
//            return parsedEnum;
//        }

//        // Default fallback
//        return UpDashUserType.None;
//    }

//}


