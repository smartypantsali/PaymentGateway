using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Framework.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentState
    {
        New,
        Completed
    }
}
