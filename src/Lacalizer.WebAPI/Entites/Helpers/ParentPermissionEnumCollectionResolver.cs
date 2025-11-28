//using UpDash.Shared.Application.Dtos.Securities.Roles;
//using UpDash.Shared.Domain.Enums.Securities;

//namespace Lacalizer.Mobile.Entites.Helpers; 
//public static class ParentPermissionEnumCollectionResolver
//{
//    public static List<ParentPermissionDto> GetParentPermissions()
//    {
//        return Enum.GetValues(typeof(ParentPermission))
//            .Cast<ParentPermission>()
//            .Where(p => p != ParentPermission.None)
//            .Select(p => new ParentPermissionDto
//            {
//                Id = p.ToString() + (int)p,
//                Name = p.ToString(),
//                Description = p.ToString(),
//                ParentId = (int)p
//            })
//            .ToList();
//    }
//}