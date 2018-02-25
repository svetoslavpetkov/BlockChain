using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Node
{
    public class ContorlerBase : Controller
    {
        protected JsonResult AsJson(object data)
        {
            JsonSerializerSettings serializerSettings =
                new JsonSerializerSettings { Formatting = Formatting.Indented };
            return Json(data, serializerSettings);
        }
    }
}
