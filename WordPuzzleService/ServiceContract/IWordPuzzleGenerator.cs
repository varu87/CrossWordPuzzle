using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;

namespace WordPuzzleService
{
    [ServiceContract]
    public interface IWordPuzzleGenerator
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GenerateGrid",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        CrossWordPuzzle GetPuzzle();
    }
}
