using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace test.Models
{
    public class Base
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("replacement")]
        public string Replacement { get; set; }
    }
    public class Value
    {
        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("rule")]
        public int Rule { get; set; }

        [JsonProperty("isTermination")]
        public bool IsTermination { get; set; }
    }


}