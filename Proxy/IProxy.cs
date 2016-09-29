using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Proxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IProxy" in both code and config file together.
    /// <summary>
    /// Service contract for the Proxy service
    /// </summary>
    [ServiceContract]
    public interface IProxy
    {
        [OperationContract]
        [WebGet(UriTemplate = "*", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Stream DoWork();

        [OperationContract]
        [WebInvoke(UriTemplate = "*", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
        Stream DoWorkPost(Stream request);
    }
}