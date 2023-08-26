using Newtonsoft.Json;

namespace TPBApi.Classes
{
    public class Response<T>
    {
        [JsonProperty("message")]
        public string? Message { get; set; }
        [JsonProperty("data")]
        public T? Data { get; set; }
    }
}
