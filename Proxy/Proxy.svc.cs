using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Proxy
{
    /// <summary>
    ///     Proxy service for the GIS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class Proxy : IProxy
    {
        #region IProxy Members

        public Stream DoWork()
        {
            try
            {
                var query = ServiceUtils.GetQuery(OperationContext.Current);
                var result = ServiceUtils.Get(WebOperationContext.Current, query);
                return result;
            }
            catch (Exception ex)
            {
                return new MemoryStream(Encoding.UTF8.GetBytes($@"{{""result"":""NOK"", ""error"":{{""code"":{"9999"}, ""message"":""{ex}""}}}}"));
            }
        }

        public Stream DoWorkPost(Stream request)
        {
            try
            {
                var query = ServiceUtils.GetQuery(OperationContext.Current);
                var parameters = ServiceUtils.ReadFullyAsString(request);
                var result = ServiceUtils.Post(WebOperationContext.Current, query, parameters);
                return result;
            }
            catch (Exception ex)
            {
                return new MemoryStream(Encoding.UTF8.GetBytes($@"{{""result"":""NOK"", ""error"":{{""code"":{"9999"}, ""message"":""{ex}""}}}}"));
            }
        }

        #endregion
    }
}