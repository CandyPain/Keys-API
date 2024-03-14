using System.Text.Json.Serialization;

namespace Keys.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Role
    {
        Teacher,
        Student,
        Unconfirmed
    }
}