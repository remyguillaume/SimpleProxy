using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Proxy
{
    /// <summary>
    ///   Class that contains Helper methods used by the Gis proxy
    /// </summary>
    internal static class ServiceUtils
    {
        public static string GetQuery(OperationContext context)
        {
            if (context.IncomingMessageProperties.Via.Query.Length <= 0)
                return String.Empty;
            
            // Remove "?" and decode string
            return Uri.UnescapeDataString(context.IncomingMessageProperties.Via.Query.Substring(1));
        }

        public static Stream Get(WebOperationContext context, string query)
        {
            Stream response;
            string contentType;

            if (SendGet(query, out response, out contentType))
            {
                // Data could be loaded from server
                if (context != null)
                {
                    context.OutgoingResponse.ContentType = contentType;
                    context.OutgoingResponse.Format = WebMessageFormat.Json;
                }

                return response;
            }

            // Data could not be retreived from server
            throw new NotImplementedException("Data could not be retreived from server");
        }

        public static Stream Post(WebOperationContext context, string query, string parameters)
        {
            Stream response;
            string contentType;

            if (SendPost(query, parameters, out response, out contentType))
            {
                // Data could be loaded from server
                if (context != null)
                {
                    context.OutgoingResponse.ContentType = contentType;
                    context.OutgoingResponse.Format = WebMessageFormat.Json;
                }

                return response;
            }

            // Data could not be retreived from server
            throw new NotImplementedException("Data could not be retreived from server");
        }

        private static bool SendGet(string query, out Stream response, out string contentType)
        {
            var request = (HttpWebRequest)WebRequest.Create(query);
            request.Method = "GET";
            request.ContentType = "text/plain";
         
            return Send(request, out response, out contentType);
        }

        private static bool SendPost(string query, string parameters, out Stream response, out string contentType)
        {
            var request = (HttpWebRequest)WebRequest.Create(query);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            request.ContentLength = parameters.Length;
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(parameters);
            }

            return Send(request, out response, out contentType);
        }

        private static bool Send(HttpWebRequest request, out Stream response, out string contentType)
        {
            using (var resp = (HttpWebResponse)request.GetResponse())
            {
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.OK:
                        contentType = resp.ContentType;
                        using (Stream responseStream = resp.GetResponseStream())
                        {
                            response = GetResponseStream(responseStream);
                            return true;
                        }
                    case HttpStatusCode.NoContent:
                        throw new NotSupportedException();
                    default:
                        response = null;
                        contentType = null;
                        return false;
                }
            }
        }

        private static MemoryStream GetResponseStream(Stream stream)
        {
            // We use a temporary MemoryStream, otherwise in some case, reading directly the stream from GetResponseStream() throws an exception.
            var buffer = new byte[1024];
            var ms = new MemoryStream();
            try
            {
                int read = stream.Read(buffer, 0, buffer.Length);
                while (read > 0)
                {
                    ms.Write(buffer, 0, read);
                    read = stream.Read(buffer, 0, buffer.Length);
                }

                ms.Position = 0;
                return ms;
            }
            catch
            {
                ms.Dispose();
                throw;
            }
        }

        public static string ReadFullyAsString(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}