using System;

namespace RapidAPISDK
{
    public class Parameter
    {
        #region Private Parameters

        private String type;
        private String value;

        #endregion Private parameters

        #region Properties

        public string Type
        {
            get { return type; }
        }

        public string Value
        {
            get { return value; }
        }
        #endregion Properties

        #region Constructor
        public Parameter(string type, string value)
        {
            this.type = type;
            this.value = value;
        }
        #endregion Constructor

    }
}
