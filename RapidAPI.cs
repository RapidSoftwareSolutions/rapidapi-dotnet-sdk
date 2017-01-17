using System;
using System.IO;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace RapidAPISDK
{
    public class RapidAPI
    {
        #region Private Parameters 
        string key;
        string project;
        #endregion Private Parameters 

        #region Private Static Functions

        /***
        * Returns the base URL for block calls
        * @returns {string} Base URL for block calls
        */
        private string getBaseURL()
        {
            return "https://rapidapi.io/connect";
        }

        /***
        * Build a URL for a block call
        * @param pack Package where the block is
        * @param block Block to be called
        * @returns {string} Generated URL
        */
        private string blockURLBuilder(string pack, string block)
        {
            return (getBaseURL() + "/" + pack + "/" + block);
        }

        #endregion Private Static Functions

        #region Public Functions

        /***
        * Creates a new RapidAPI Connect instance
        * @param project Name of the project you are working with
        * @param key API key for the project
        */
        public RapidAPI(string project, string key)
        {
            this.project = project;
            this.key = key;
        }


        /***
        * Call a block
        * @param pack Package of the block
        * @param block Name of the block
        * @param args Arguments to send to the block (JSON)
        */
        public Dictionary<String, Object> call(string pack, string block, object args)
        {
            Dictionary<String, Object> result = new Dictionary<String, Object>();
            var client = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            foreach (var pair in (Dictionary<string, Parameter>)args)
            {
                if (pair.Value.Type == "data")
                {
                    form.Add(new StringContent(pair.Value.Value), pair.Key);
                }
                else
                {
                    if (File.Exists(pair.Value.Value))
                    {
                        FileStream fileStream = new FileStream(pair.Value.Value, FileMode.Open, FileAccess.Read);
                        StreamContent streamContent = new StreamContent(fileStream);
                        form.Add(streamContent, pair.Key, pair.Value.Value);
                    }
                    else
                    {
                        result.Add("error", "File not exist or can't be read.");
                        return result;
                    }
                }
            }

            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
                    string.Format("{0}:{1}", this.project, this.key))));
            var response = client.PostAsync(blockURLBuilder(pack, block), form).Result;
            var contents = response.Content.ReadAsStringAsync().Result;
            try
            {
                Dictionary<String, Object> map = new Dictionary<String, Object>();
                map = JsonConvert.DeserializeObject<Dictionary<string, object>>(contents);

                // return payload
                object payload, outcome;
                map.TryGetValue("payload", out payload);
                map.TryGetValue("outcome", out outcome);

                if (!response.IsSuccessStatusCode || outcome.Equals("error"))
                {
                    result.Add("error", payload);
                    return result;
                }

                result.Add("success", payload);
                return result;
            }
            catch 
            {
                result.Add("error", contents);
                return result;
            }
        }

        #endregion Public Functions

    }
}