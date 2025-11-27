using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NUlid;

namespace Lacalizer.WebAPI.Entites.Helpers.Converters
{
    public class NullableUlidToStringConverter : ValueConverter<Ulid?, string>
    {
        public NullableUlidToStringConverter()
            : base(
                ulid => ulid.HasValue ? ulid.Value.ToString() : null,
                str => string.IsNullOrEmpty(str) ? null : Ulid.Parse(str))
        {
        }
    }
}
