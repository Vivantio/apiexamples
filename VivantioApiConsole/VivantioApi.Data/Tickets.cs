﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VivantioApi.Model;

namespace VivantioApi.Data
{
    public static class Tickets
    {
        /// <summary>
        /// Get Single Ticket Info By Id
        /// </summary>
        public static async Task<TicketType> GetSingleTicketByIdAsync(int ticketId)
        {
            var path = $"Ticket/SelectById?id={ticketId}";
            HttpResponseMessage response = await HttpClientProvider.Client.PostAsync(path, null).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<ResponseItem<TicketType>>(data).Item;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get All Users of the system
        /// </summary>
        public static async Task<List<UserInfo>> GetAllUsers()
        {
            const string path = "Configuration/UserSelectAll";
            HttpResponseMessage response = await HttpClientProvider.Client.PostAsync(path, null).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<ResponseList<UserInfo>>(data).Results;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Get All Actions By TicketId
        /// </summary>
        public static async Task<List<TicketAction>> GetAllActionsByTicketId(int ticketTypeId, bool includePrivate, bool includeAttachments)
        {
            var path = $"Ticket/ActionSelectByParentId/{ticketTypeId}?includePrivate={includePrivate}&includeAttachments={includeAttachments}";

            HttpResponseMessage response = await HttpClientProvider.Client.PostAsync(path, null).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<ResponseList<TicketAction>>(data).Results;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Get List of Child Tickets By Id
        /// </summary>
        public static async Task<List<TicketType>> GetChildTicketById(int ticketId)
        {
            var path = $"Ticket/SelectChildTicketsByTicketId?id={ticketId}";
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

        /// <summary>
        /// Get List of Tickets
        /// </summary>
        public static async Task<List<TicketType>> GetListofTickets(Array tickets)
        {
            const string path = "Ticket/SelectList";

            HttpContent content = new StringContent(JsonConvert.SerializeObject(tickets), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HttpClientProvider.Client.PostAsync(path, content).ConfigureAwait(false);
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
    }
}