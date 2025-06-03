using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VivantioApi.Model;

namespace VivantioApi.Data
{
    public static class CustomForms
    {
        public static async Task<List<TicketType>> GetTicketTypeAsync()
        {
            const string path = "Configuration/TicketTypeSelectAll";

            HttpResponseMessage response = await HttpClientProvider.Client.PostAsync(path, null).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<ResponseList<TicketType>>(data).Results;
            }
            else
            {
                return null;
            }
        }

        public static async Task<List<FormDefinition>> GetCustomFormDefinitionForTicketTypeAsync(int ticketTypeId)
        {
            const string path = "Entity/CustomEntityDefinitionSelectByRecordTypeId";

            var jsonParameters = new
            {
                Id = ticketTypeId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(jsonParameters), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HttpClientProvider.Client.PostAsync(path, content).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<ResponseList<FormDefinition>>(data).Results;
            }
            else
            {
                return null;
            }
        }

        public static async Task<FormDefinition> GetCustomFormDefinitionDetailForFormDefinitionIdAsync(int formDefinitionId)
        {
            var path = $"Entity/CustomEntityDefinitionSelectById?id={formDefinitionId}";

            HttpResponseMessage response = await HttpClientProvider.Client.PostAsync(path, null).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<ResponseItem<FormDefinition>>(data).Item;
            }
            else
            {
                return null;
            }
        }

        public static async Task<List<string>> GetCustomFormDefinitionForTicketInstanceAsync(int ticketInstanceId, string systemArea)
        {
            const string path = "Entity/SelectEntityTypeIdsByParentItem";

            var jsonParameters = new
            {
                ParentId = ticketInstanceId,
                SystemArea = systemArea
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(jsonParameters), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HttpClientProvider.Client.PostAsync(path, content).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var json = (JObject)JsonConvert.DeserializeObject(data);
                var results = json["Results"].Value<JArray>();

                return results.ToObject<List<string>>();
            }
            else
            {
                return null;
            }
        }

        public static async Task<FieldDefinition> GetCustomFieldDefinitionAsync(int fieldDefinitionId)
        {
            var path = $"Entity/CustomEntityFieldDefinitionSelectById/{fieldDefinitionId}";

            HttpResponseMessage response = await HttpClientProvider.Client.PostAsync(path, null).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<ResponseItem<FieldDefinition>>(data).Item;
            }
            else
            {
                return null;
            }
        }

        public static async Task<List<FormInstance>> GetCustomFormInstanceForTicketInstanceAsync(int typeId, int ticketInstanceId, string systemArea)
        {
            const string path = "Entity/CustomEntitySelectByTypeIdAndParent";

            var jsonParameters = new
            {
                TypeId = typeId,
                ParentId = ticketInstanceId,
                SystemArea = systemArea
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(jsonParameters), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HttpClientProvider.Client.PostAsync(path, content).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<ResponseList<FormInstance>>(data).Results;
            }
            else
            {
                return null;
            }
        }
    }
}
