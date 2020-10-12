using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace WEB.Extensions
{
    public static class HttpRequestExtension
    {
        public static string GetActionAbsoluteUrl(this HttpRequest request, string action, Controller controller)
        {
            if (request == null || string.IsNullOrEmpty(action) || controller == null)
            {
                return null;
            }

            return controller.Url.Action(action, null, null, request.Scheme);
        }

        /// <summary>
        /// Gets absolute Uri to application root folder.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ApplicationBaseUrl(this HttpRequest request)
        {
            if (request == null)
            {
                return null;
            }

            return $"{request.Scheme}://{request.Host.ToUriComponent()}{request.PathBase.ToUriComponent()}";
        }

        /// <summary>
        /// Gets the raw target of an HTTP request.
        /// </summary>
        /// <returns>Raw target of an HTTP request</returns>
        /// <remarks>
        /// ASP.NET Core manipulates the HTTP request parameters exposed to pipeline
        /// components via the HttpRequest class. This extension method delivers an untainted
        /// request target. https://tools.ietf.org/html/rfc7230#section-5.3
        /// </remarks>
        public static string GetRawTarget(this HttpRequest request)

        {
            var httpRequestFeature = request.HttpContext.Features.Get<IHttpRequestFeature>();
            return httpRequestFeature.RawTarget;
        }

        public static string GetRequestUrl(this HttpRequest request)
        {
            if (request == null)
            {
                return null;
            }

            return request.ApplicationBaseUrl() + request.GetEncodedPathAndQuery();
        }
    }
}
