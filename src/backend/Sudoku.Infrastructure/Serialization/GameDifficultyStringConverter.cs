using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sudoku.Infrastructure.Serialization;

/// <summary>
/// Serializes a game difficulty as a plain string. On read it also accepts the legacy
/// value-object shape (<c>{ "value": n, "name": "..." }</c>) that older documents persisted,
/// returning the difficulty name so existing saved games remain loadable.
/// </summary>
public class GameDifficultyStringConverter : JsonConverter<string?>
{
    public override string? ReadJson(JsonReader reader, Type objectType, string? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        switch (reader.TokenType)
        {
            case JsonToken.Null:
                return null;
            case JsonToken.String:
                return (string?)reader.Value;
            case JsonToken.StartObject:
                var obj = JObject.Load(reader);
                var name = obj["name"] ?? obj["Name"];
                return name?.Value<string>();
            default:
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when reading difficulty.");
        }
    }

    public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
    {
        writer.WriteValue(value);
    }
}
