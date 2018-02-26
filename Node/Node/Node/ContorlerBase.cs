using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Node
{
    public class ContorlerBase : Controller
    {
        protected JsonResult AsJson(object data)
        {
            JsonSerializerSettings serializerSettings =
                new JsonSerializerSettings { Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return Json(data, serializerSettings);
        }
    }
}
