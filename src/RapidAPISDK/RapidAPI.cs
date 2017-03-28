using System;
using System.IO;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.WebSockets;
using RapidAPISDK.Events;
using Phoenix;

namespace RapidAPISDK
{
	
    public class RapidAPI
    {
		public event MessageCallback OnMessage;
		public event TimeOutCallback OnTimeOut;
		public event ErrorCallback OnError;
		public event ConnectCallback OnConnect;
        #region Private Static Functions

        private const string BaseUrl = "https://rapidapi.io/connect";

		private const string WebHooksBaseUrl = "https://webhooks.rapidapi.com";
		private const string WebSocketBaseUrl = "wss://webhooks.rapidapi.com";
        /***
        * Build a URL for a block call
        * @param pack Package where the block is
        * @param block Block to be called
        * @returns {string} Generated URL
        */
        private string BuildBlockUrl(string pack, string block)
        {
            return $"{BaseUrl}/{pack}/{block}";
        }


        /**
         * 
         */
         private static Dictionary<String, object> BuildResponse(string content, bool IsSuccessStatus)
        {
            Dictionary<String, object> result = new Dictionary<String, Object>();

            try
            {
                Dictionary<String, Object> map = new Dictionary<String, Object>();
                map = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                object payload, outcome, payloadObj;
                map.TryGetValue("payload", out payload);
                map.TryGetValue("outcome", out outcome);
                if (payload.GetType() == typeof(string))
                {
                    try
					{
 
                        payloadObj = JsonConvert.DeserializeObject<object>(payload.ToString());
                    }
                    catch
                    {
                        payloadObj = payload;
                    }
                }
                else
                {
                    payloadObj = payload;
                }

                if (!IsSuccessStatus || outcome.Equals("error"))
                {
                    result.Add("error", payloadObj);
                    return result;
                }

                result.Add("success", payloadObj);
                return result;
            }
            catch (RapidAPIServerException)
            {
                throw;
            }
            catch (Exception)
            {
                result.Add("error", content);
                return result;
            }
        }
         
        #endregion 

        #region Private Members 

        private readonly HttpClient _Client;

		private readonly HttpClient _WebHooksClient;

		private string _project;

		private string _key;

        #endregion 

        #region C'tor


        /***
        * Creates a new RapidAPI Connect instance
        * @param project Name of the _project you are working with
        * @param key API _key for the project
        */
        public RapidAPI(string project, string key)
        {
			_project = project;

			_key = key;

            _Client = new HttpClient();

            _Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

            var auth = Encoding.ASCII.GetBytes($"{project}:{key}");

            _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(auth));


			_WebHooksClient = new HttpClient();

			_WebHooksClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


			_WebHooksClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(auth));

        }

        #endregion


        #region Public Functions

        /***
        * Call a block
        * @param pack Package of the block
        * @param block Name of the block
        * @param parameters Arguments to send to the block (JSON)
        */
        public async Task<Dictionary<string, object>> Call(string pack, string block, params Parameter[] parameters)
        {
            return await CallAsync<object>(pack, block, parameters);
        }

        /***
       * Call a block
       * @param pack Package of the block
       * @param block Name of the block
       * @param parameters Arguments to send to the block (JSON)
       */
        private async Task<Dictionary<string, object>> CallAsync<T>(string pack, string block, params Parameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(pack)) throw new ArgumentException("Cannot be null or empty", nameof(pack));
            if (string.IsNullOrWhiteSpace(block)) throw new ArgumentException("Cannot be null or empty", nameof(block));
            if (parameters == null) parameters = new Parameter[0];

            MultipartFormDataContent form = new MultipartFormDataContent();

            foreach (var parameter in parameters)
                parameter.AddToContent(form);

            var response = await _Client.PostAsync(BuildBlockUrl(pack, block), form);
            var content = await response.Content.ReadAsStringAsync();
            return BuildResponse(content, response.IsSuccessStatusCode);
        }
		/***
	     * Listen for webhook events
	     * @param pack Package of the event
	     * @param event Name of the event
	     * @param parameters params for API provider
	     */
		public void Listen(string pack, string block, params Parameter[] parameters)
		{
			string user_id = String.Format("{0}.{1}_{2}:{3}", pack, block, _project, _key);
			var token = GetToken(user_id).Result;
			string socket_url = string.Format("{0}/socket", WebSocketBaseUrl);
			Console.Write(socket_url);
			var param = new Newtonsoft.Json.Linq.JObject();
			param["token"] = token;
			var options = new Phoenix.SocketOptions()
			{
				Params = param
			};

			Socket socket = new Socket(socket_url,options);
			socket.Connect();
			var data = new Newtonsoft.Json.Linq.JObject();
			for (int i = 0; i < parameters.Length; i++)
			{
				data[parameters[i].Key] = ((DataParameter)parameters[i]).Value;
			}
			var channel = socket.Channel("users_socket:" +token, data);
			channel.On("new_msg", (jo, x) => OnMessage(jo["body"].ToString()));
			channel.Join()
			       .Receive("ok", (jo) => OnConnect())
				   .Receive("error", (jo) => OnError(jo["body"].ToString()))
			       .Receive("timeout", (jsonobj) => OnTimeOut())
			               ;
		}
		 /***
       * get a token from RapidAPI to use webhooks
       * @param userID
       */
		private async Task<string> GetToken(string userID)
		{
			var response = await _WebHooksClient.GetAsync(string.Format("{0}/api/get_token?user_id={1}&user={2}&pass={3}", WebHooksBaseUrl, userID,_project,_key));
			string content = await response.Content.ReadAsStringAsync();
			var token = JsonConvert.DeserializeObject<RapidAPIServerResponse>(content).token;
			return token;
		}

        #endregion 

        }
}