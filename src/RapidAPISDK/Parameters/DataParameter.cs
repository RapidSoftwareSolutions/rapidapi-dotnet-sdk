using System.Net.Http;

namespace RapidAPISDK
{
    public class DataParameter : Parameter
    {

        #region C'tor

        public DataParameter()
        {
        }

        public DataParameter(string key) : base(key)
        {
        }

        public DataParameter(string key, string value) : base(key)
        {
            Value = value;
        }

        #endregion

        #region Public Properties

        public string Value { get; set; }

        #endregion

        #region Overrides of Parameter

        public override void AddToContent(MultipartFormDataContent content)
        {
            content.Add(new StringContent(Value ?? string.Empty), Key);
        }

        #endregion
    }
}