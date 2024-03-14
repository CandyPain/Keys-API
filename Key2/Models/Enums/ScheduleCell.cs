using System.Text.Json.Serialization;

namespace Keys.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ScheduleCell
    {
        First,
        Second,
        Third,
        Fourth,
        Fifth,
        Sixth,
        Seventh
    }
}