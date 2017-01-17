using RapidAPISDK;
using System.Collections.Generic;

namespace test
{
    class test
    {
        #region Helpers

        private static void getPayload( Dictionary<string, object> res )
        {
            object payload;
            res.TryGetValue("success", out payload);
            if (payload != null)
                System.Console.WriteLine("success: " + payload);
            else 
            {
                res.TryGetValue("error", out payload);
                System.Console.WriteLine("error: " + payload);
            }

            System.Console.ReadKey();
        }

        #endregion Helpers

        #region Tests

        private static void testPublicPack()
        {
            RapidAPI rapidApi = new RapidAPI("projectName", "********");
            var pairs = new Dictionary<string, Parameter>();
            pairs.Add("apiKey", new Parameter("data", ""));
            pairs.Add("date", new Parameter("data", ""));
            pairs.Add("highResolution", new Parameter("data", ""));
           
            var res = rapidApi.call("NasaAPI", "getPictureOfTheDay", pairs);
            getPayload(res);

        }

        private static void testPack()
        {
            RapidAPI rapidApi = new RapidAPI("projectName", "********");
            var pairs = new Dictionary<string, Parameter>();
            pairs.Add("apiKey", new Parameter("data", "********"));
            pairs.Add("string", new Parameter("data", "מבחן"));
            pairs.Add("targetLanguage", new Parameter("data", "en"));
            pairs.Add("sourceLanguage", new Parameter("data", ""));

            var res = rapidApi.call("GoogleTranslate", "translateAutomatic", pairs);
            getPayload(res);
        }


        private static void testPackWithImg()
        {
            RapidAPI rapidApi = new RapidAPI("projectName", "********");
            var pairs = new Dictionary<string, Parameter>();
            pairs.Add("subscriptionKey", new Parameter("data", "********"));
            pairs.Add("image", new Parameter("data", "http://cdn.litlepups.net/2015/08/31/cute-dog-baby-wallpaper-hd-21.jpg"));
            //pairs.Add("image", new Parameter("file", @"FilePath"));
            pairs.Add("width", new Parameter("data", "50"));
            pairs.Add("height", new Parameter("data", "50"));
            pairs.Add("smartCropping", new Parameter("data", ""));

            var res = rapidApi.call("MicrosoftComputerVision", "analyzeImage", pairs);
            getPayload(res);

        }

        #endregion Tests


        static void Main(string[] args)
        {
            testPublicPack();
            testPackWithImg();
            testPack();

        }
    }
}
