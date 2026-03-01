using Microsoft.AspNetCore.Mvc;

namespace MicroondasDigital.Utils;

class JsonValidationResult(object data) : JsonResult(data)
{
    public bool IsValid { get; set; }

    public static JsonValidationResult CreateError(string html, string message = "")
    {
        var json = new JsonValidationResult(data: new { success = false, 
                                                        message = message, 
                                                        html = html 
                                                      });
        json.IsValid = false;
        
        return json;
    }

    public static JsonValidationResult CreateSuccess()
    {
        var json = new JsonValidationResult(data: new { success = true });
        json.IsValid = true;

        return json;
    }
}