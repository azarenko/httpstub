using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace HttpStub
{
    /// <summary>
    /// Single handler that accepts all HTTP requests and echoes request data as JSON.
    /// </summary>
    public class EchoHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;

            // Read raw body
            string body = ReadRequestBody(request);

            var payload = new
            {
                method = request.HttpMethod,
                path = request.Url.AbsolutePath,
                queryString = request.Url.Query,
                scheme = request.Url.Scheme,
                host = request.Headers["Host"],
                protocol = request.ServerVariables["SERVER_PROTOCOL"],
                contentType = request.ContentType,
                contentLength = request.ContentLength,
                remoteIp = request.UserHostAddress,
                url = request.Url.ToString(),
                headers = ToDictionary(request.Headers),
                query = ToDictionary(request.QueryString),
                body
            };

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(payload);

            response.StatusCode = 200;
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.Write(json);
        }

        private static string ReadRequestBody(HttpRequest request)
        {
            if (request.InputStream == null || !request.InputStream.CanRead)
            {
                return string.Empty;
            }

            try
            {
                if (request.InputStream.CanSeek)
                {
                    request.InputStream.Position = 0;
                }

                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true))
                {
                    var body = reader.ReadToEnd();

                    // Reset position so other components (if any) can read it again
                    if (request.InputStream.CanSeek)
                    {
                        request.InputStream.Position = 0;
                    }

                    return body;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private static IDictionary<string, string[]> ToDictionary(NameValueCollection collection)
        {
            if (collection == null)
            {
                return new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            }

            return collection.AllKeys
                .Where(k => k != null)
                .ToDictionary(
                    k => k,
                    k => collection.GetValues(k) ?? Array.Empty<string>(),
                    StringComparer.OrdinalIgnoreCase);
        }
    }
}

