using Lacalizer.Mobile.Entites.Enums;

namespace Lacalizer.Mobile.Entites.Helpers;
public static class UIDHelperGenerator
{
    public static string GenerateUserCode(EntityTypeEnum typeEnum)
    {
        var uniqueDigits = new Random().Next(100000, 999999); // 6-digit number
       string prefix =  string.Empty;
       string suffix =  string.Empty;

        switch (typeEnum)
        {
            case EntityTypeEnum.VideoItem:
                prefix = "LCL";
                suffix = "VD";
                break;
            //case UpDashUserType.UpdashManagerUser:
            //    prefix = "MCHT";
            //    suffix = "MA";
            //    break;
            //case UpDashUserType.UpdashStoreUser:
            //    prefix = "MCHT";
            //    suffix = "SM";
            //    break;
            //case UpDashUserType.UpdashDriverUser:
            //    prefix = "UDDRVER";
            //    break;
            //case UpDashUserType.UpdashInternalAdminUser:
            //    prefix = "UDADMIN";
            //    break;
            //case UpDashUserType.UpdashCustomerUser:
            //    prefix = "UDCUST";
            //    break;
            //case UpDashUserType.UpDashSuperAdmin:
            //    prefix = "UDSUPERADMIN";
            //    uniqueDigits = new Random().Next(1, 100);  
            //    break;
            default:
                if (!string.IsNullOrWhiteSpace(prefix) && !string.IsNullOrWhiteSpace(prefix))
                    break;
                else
                    throw new ArgumentException("Unsupported user type.");
        }

        return $"{prefix}{uniqueDigits}{suffix}";
    }
    
}
