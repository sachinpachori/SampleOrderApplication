using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Order.BE.Business.Helpers
{
    public static class HttpLoggerHelper
    {
        public static string FormatResponseWithoutBodyForLog(HttpResponse response)
        {
            var log = string.Format("traceId: {0}, type: Response, statusCode: {1}",
               response.HttpContext.TraceIdentifier,
               response.StatusCode
               );

            return log;
        }

        public static string FormatResponseWithBodyForLog(HttpResponse response, string responseBody)
        {
            var log = string.Format("traceId: {0}, type: Response, statusCode: {1}, body: {2}",
                            response.HttpContext.TraceIdentifier,
                            response.StatusCode,
                            CleanStringForLog(responseBody)
                            );

            return log;
        }

        public static string FormatRequestWithoutBodyForLog(HttpRequest request)
        {
            var log = string.Format("traceId: {0}, type: Request, schema: {1}, host: {2}, path: {3}, query: {4}",
                request.HttpContext.TraceIdentifier,
                request.Scheme,
                request.Host.Value,
                request.Path,
                CleanStringForLog(request.QueryString.Value)
            );

            return log;
        }

        public static string FormatRequestWithBodyForLog(HttpRequest request, string requestBody)
        {
            var log = string.Format("traceId: {0}, type: Request, schema: {1}, host: {2}, path: {3}, query: {4}, body: {5}",
                            request.HttpContext.TraceIdentifier,
                            request.Scheme,
                            request.Host.Value,
                            request.Path,
                            CleanStringForLog(request.QueryString.Value),
                            CleanStringForLog(requestBody)
            );

            return log;
        }

        public static string FormatExceptionForLog(HttpContext context, Exception ex)
        {
            var log = string.Format("traceId: {0}, type: Error, message: {1}, stack: ",
                context.TraceIdentifier,
                ex.Message
                );

            return log;
        }

        private static string CleanStringForLog(string s)
        {
            s = s.Replace("\r", string.Empty);
            s = s.Replace("\n", string.Empty);

            return string.IsNullOrWhiteSpace(s) ? "none" : s;
        }
    }
}
