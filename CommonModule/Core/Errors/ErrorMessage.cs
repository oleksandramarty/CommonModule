using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CommonModule.Core.Errors;

public class ErrorMessage
{
    public ErrorMessage() { }

    public ErrorMessage(string message, int statuscode)
    {
        Message = message;
        StatusCode = statuscode;
    }

    public ErrorMessage(string message, int statuscode, IReadOnlyCollection<InvalidFieldInfo> invalidFields)
    {
        Message = message;
        StatusCode = statuscode;
        InvalidFields = invalidFields;
    }

    public IReadOnlyCollection<InvalidFieldInfo> InvalidFields { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }

    public string ToJson()
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        return JsonConvert.SerializeObject(this, settings);
    }

    public static ErrorMessage FromJson(string data)
    {
        return JsonConvert.DeserializeObject<ErrorMessage>(data);
    }
}