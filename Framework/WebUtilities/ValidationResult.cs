using Framework.WebUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.WebUtilities
{
    /// <summary>
    /// Class provides a cleaner way to return object structured error messages
    /// </summary>
    public class ValidationResult : ActionResult
    {
        public byte[] Content { get; set; }
        public int StatusCode { get; set; }

        public ValidationResult()
        {
        }

        /// <summary>
        /// Unifies all ValidationResults into one dictionary and returns that in the response
        /// </summary>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        public static ValidationResult GetUnifiedTeapotValidationResults(IEnumerable<ValidationResult> validationResults)
        {
            if (validationResults == null || validationResults.Count() == 0)
            {
                return null;
            }

            var res = new ValidationResult();

            var obj = new Dictionary<string, object>();

            for (int i = 0; i < validationResults.Count(); i++)
            {
                var validationResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(UTF8Encoding.UTF8.GetString(validationResults.ElementAt(i).Content));

                obj[$"{validationResult.ElementAt(0).Key}"] = validationResult.ElementAt(0).Value;
            }

            res.Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            res.StatusCode = 418;

            return res;
        }

        /// <summary>
        /// Forms 418 client error response object with response object
        /// </summary>
        /// <param name="offense"></param>
        /// <param name="propertName"></param>
        /// <returns></returns>
        public static ValidationResult ToTeapotResult(ApiOffence offense, string propertName)
        {
            if (offense.Equals(default(ApiOffence)) || propertName == null || propertName.Length == 0)
            {
                return null;
            }

            var res = new ValidationResult();
            res.Content = getContent(offense, propertName);
            res.StatusCode = 418;
            return res;
        }

        /// <summary>
        /// Generates response object for ApiOffence
        /// </summary>
        /// <param name="offense"></param>
        /// <param name="propertName"></param>
        /// <returns></returns>
        private static byte[] getContent(ApiOffence offense, string propertName)
        {
            var obj = new Dictionary<string, object>()
            {
                [propertName] = new { ErrorCode = offense.ErrorCode }
            };

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// Override default result method to return custom object
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ExecuteResultAsync(ActionContext context)
        {
            var resp = context.HttpContext.Response;
            resp.StatusCode = StatusCode;

            if (Content != null)
            {
                resp.ContentType = "application/json";
                return resp.Body.WriteAsync(Content, 0, Content.Length);
            }

            return Task.CompletedTask;
        }
    }
}
