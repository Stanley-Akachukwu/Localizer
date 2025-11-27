//using UpDash.Shared.Domain.Enums.Securities;
//using UpDash.Shared.Domain.Securities.Roles;

//namespace Lacalizer.Mobile.Entites.Helpers;


//public static class PermissionConfigFilter
//{
//    public static Task<List<PermissionConfig>> FilterPermissionConfigs(
//        UpDashUserType userType,
//        List<PermissionConfig> permissionConfigs)
//    {
//                var predicateMap = new Dictionary<UpDashUserType, Func<PermissionConfig, bool>>
//                {
//                    { UpDashUserType.UpDashSuperAdmin, p => p.UpDashSuperAdminAssign == true },
//                    { UpDashUserType.UpdashInternalAdminUser, p => p.UpDashAdminAssign == true },
//                    { UpDashUserType.MerchantSuperAdmin, p => p.MerchantAssign == true },
//                    { UpDashUserType.UpdashManagerUser, p => p.ManagerUserAssign == true },
//                    { UpDashUserType.UpdashStoreUser, p => p.StoreUserAssign == true },
//                    { UpDashUserType.UpdashDriverUser, p => p.DriverAssign == true },
//                    { UpDashUserType.UpdashCustomerUser, p => p.CustomerAssign == true }
//                };


//        if (predicateMap.TryGetValue(userType, out var predicate))
//        {
//            permissionConfigs = permissionConfigs.Where(predicate).ToList();
//        }

//        return Task.FromResult(permissionConfigs);
//    }
//    private static List<PermissionConfig> RemovePermissions(
//       List<PermissionConfig> configs,
//       List<int> permissionIdsToRemove)
//    {
//        return configs.Where(p => !permissionIdsToRemove.Contains(p.PermissionId)).ToList();
//    }
//    public static async Task<List<PermissionConfig>> FilterContractorPermissionConfigs(List<PermissionConfig> permissionConfigs)
//    {
//        permissionConfigs = RemovePermissions(permissionConfigs, new List<int>
//            {
//                (int)Permission.CanScheduleWorkingDays,
//                (int)Permission.CanViewSchedules,
//                (int)Permission.CanViewDeliveryFees
//            });
//        return permissionConfigs;
//    }
//    public static async Task<List<PermissionConfig>> FilterTier1And2PermissionConfigs(List<PermissionConfig> permissionConfigs)
//    {
//        permissionConfigs = RemovePermissions(permissionConfigs, new List<int>
//            {
//                (int)Permission.CanViewTodayDashboard,
//                (int)Permission.CanViewStores,
//                (int)Permission.CanAddStore,
//                (int)Permission.CanEditStore,
//                (int)Permission.CanViewPerformance,
//                (int)Permission.CanViewCustomerInsights,
//                (int)Permission.CanViewOrders,
//                (int)Permission.CanManageCalendar,
//                (int)Permission.CanManageHolidayCalender,
//                (int)Permission.CanViewCalendar,
//                (int)Permission.CanManageDeviceSettings,
//                (int)Permission.CanAddMenu,
//                (int)Permission.CanDeleteMenu,
//                (int)Permission.CanViewMenu,
//                (int)Permission.CanManageIssues,
//                (int)Permission.CanManageMarketing,
//                (int)Permission.CanViewMarketingPerformance
//            });
//        return permissionConfigs;
//    }
//    public static RolePermission MapToRolePermission(PermissionConfig permission, string roleId, string? createdBy) =>
//          new()
//          {
//              IsActive = true,
//              CreatedByUserId = createdBy,
//              DateCreated = DateTime.Now,
//              DateUpdated = DateTime.Now,
//              Description = permission.Description,
//              Id = Ulid.NewUlid().ToString(),
//              PermissionId = permission.PermissionId,
//              PermissionName = permission.Name,
//              RoleId = roleId,
//              ParentId = permission.ParentId.Value,
//              ParentName = permission.ParentName,
//          };
//}
