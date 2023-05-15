using System.Text.Json;
using System.Text.Json.Serialization;
using static Hatebook.Models.HatebookMainModel;

namespace Hatebook.Common
{
    public class GenderConverter : JsonConverter<Gender>
    {
        public override Gender Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Enum.Parse<Gender>(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, Gender value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
