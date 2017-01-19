using System;
using System.Net.Http;

namespace RapidAPISDK
{
    public abstract class Parameter
    {

        #region C'tor

        protected Parameter()
        {
        }

        protected Parameter(string key)
        {
            Key = key;
        }

        #endregion

        #region Public Proeprties

        public string Key { get; set; }

        #endregion

        #region Abstract Methods

        public abstract void AddToContent(MultipartFormDataContent content);

        #endregion

    }
}