using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Framework.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Permission
    {
        Payment_View = 1 << 0,
        Payment_Create = 1 << 1
    }
}
