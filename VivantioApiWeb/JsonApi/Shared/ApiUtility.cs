using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Vivantio.Samples.JsonApi.Shared
{
    public class ApiUtility
    {
        public const string PlatformUrlEnvVar = "VIVANTIO_PLATFORM_URL";
        public const string UsernameEnvVar = "VIVANTIO_USERNAME";
        public const string PasswordEnvVar = "VIVANTIO_PASSWORD";

        public string ApiUrl { get; set; }
        public string ApiUser { get; set; }
        public string ApiPassword { get; set; }

        public ApiUtility()
        {
            ApiUrl = $"{Environment.GetEnvironmentVariable(PlatformUrlEnvVar, EnvironmentVariableTarget.User)}api/";
            ApiUser = Environment.GetEnvironmentVariable(UsernameEnvVar, EnvironmentVariableTarget.User);
            ApiPassword = Environment.GetEnvironmentVariable(PasswordEnvVar, EnvironmentVariableTarget.User);
        }

        public TResponse SendRequest<TResponse>(string path)
        {
            return SendRequest<TResponse, object>(path, null);
        }

        public TResponse SendRequest<TResponse, TRequest>(string path, TRequest request)
            where TRequest : class
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(ApiUrl + path);

            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";

            AddCredentialToRequest(httpRequest);

            if (request != null)
            {
                var serialized = JsonConvert.SerializeObject(request);
                using (var sw = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    sw.WriteLine(serialized);
                }
            }
            else
                httpRequest.ContentLength = 0;

            var response = (HttpWebResponse)httpRequest.GetResponse();

            var rs = response.GetResponseStream();
            if (rs != null)
            {
                using (var sr = new StreamReader(rs))
                {
                    var s = sr.ReadToEnd();
                    return JsonConvert.DeserializeObject<TResponse>(s);
                }
            }

            return default;
        }

        private void AddCredentialToRequest(HttpWebRequest httpRequest)
        {
            // http://stackoverflow.com/questions/1702426/httpwebrequest-not-passing-credentials
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(ApiUser + ":" + ApiPassword));
            httpRequest.Headers.Add("Authorization", "Basic " + credentials);
        }
    }
}