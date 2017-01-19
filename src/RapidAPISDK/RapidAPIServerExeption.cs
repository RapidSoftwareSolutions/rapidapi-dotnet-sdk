using System;

namespace RapidAPISDK
{

    public class RapidAPIServerException : Exception
    {

        #region C'tor

        public RapidAPIServerException(object error) : base(error.ToString())
        {
        }

        #endregion

        #region Properties

        public object Error { get; }

        #endregion

        #region Overrides of Exception

        public override string ToString()
        {
            return $"Error: {Error} {Environment.NewLine} {base.ToString()}";
        }

        #endregion
    }

}