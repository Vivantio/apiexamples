using System.Net;
using Vivantio.Samples.JsonApi.Shared;

namespace Vivantio.Samples.JsonApi
{
    public abstract class BaseApi
    {
        static BaseApi()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        private readonly ApiUtility _utility;

        protected ApiUtility ApiUtility
        {
            get { return _utility; }
        }

        protected BaseApi()
        {
            _utility = new ApiUtility();
        }
    }
}