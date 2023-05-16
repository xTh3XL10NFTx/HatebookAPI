using System.Text.Json;
using System.Text.Json.Serialization;
using static Hatebook.Models.HatebookMainModel;

namespace Hatebook.Common
{
    public class GenderConverter : JsonConverter<Gender>
    {
        public override Gender Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string genderValue = reader.GetString();

                if (!string.IsNullOrEmpty(genderValue))
                {
                    if (Enum.TryParse<Gender>(genderValue, out Gender gender))
                    {
                        return gender;
                    }
                }
            }

            // Gender is not specified or invalid, return unknown
            return Gender.Unknown;
        }

        public override void Write(Utf8JsonWriter writer, Gender value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
