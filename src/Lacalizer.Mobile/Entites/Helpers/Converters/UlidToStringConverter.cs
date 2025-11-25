using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NUlid;

namespace Lacalizer.Mobile.Entites.Helpers.Converters
{
    public class UlidToStringConverter : ValueConverter<Ulid, string>
    {
        public UlidToStringConverter()
            : base(
                ulid => ulid.ToString(),
                str => Ulid.Parse(str))
        {
        }
    }
}
