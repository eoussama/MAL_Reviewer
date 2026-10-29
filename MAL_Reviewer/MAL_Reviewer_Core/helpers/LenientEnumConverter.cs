using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// Writes a setting's value as its name, and reads names or numbers; an unknown value means
    /// the enumeration's first value rather than making the whole settings file unreadable.
    /// </summary>
    /// <typeparam name="TEnum"> The enumeration of the setting. </typeparam>
    internal class LenientEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && Enum.TryParse(reader.GetString(), true, out TEnum value) && Enum.IsDefined(value))
            {
                return value;
            }

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int number))
            {
                TEnum numbered = (TEnum)Enum.ToObject(typeof(TEnum), number);

                if (Enum.IsDefined(numbered))
                {
                    return numbered;
                }
            }

            reader.Skip();

            return default;
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
