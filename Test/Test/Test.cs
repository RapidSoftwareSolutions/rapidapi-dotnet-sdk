using System;
using RapidAPISDK;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace RapidTestTest
{

    class Test
    {

        #region Helpers

        private static RapidAPI RapidApi = new RapidAPI("MyRapidTest", "*****");

        private static Dictionary<string, object> Call(string pack, string block, params Parameter[] parameters)
        {
            try
            {
                var res = RapidApi.Call(pack, block, parameters).Result;

                Console.WriteLine(res);
                object payload;
                if (res.TryGetValue("success", out payload))
                    Console.WriteLine("success: " + payload);
                else
                {
                    res.TryGetValue("error", out payload);
                    Console.WriteLine("error: " + payload);
                }

                return res;
            }
            catch (RapidAPIServerException e)
            {
                Console.WriteLine("Server error: " + e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown exeption: " + e);
            }
            return null;
        }

        #endregion Helpers

        #region Tests

        private static void TestPublicPack()
        {
            var args = new List<Parameter>
                       {
                           new DataParameter("apiKey"),
                           new DataParameter("date"),
                           new DataParameter("highResolution")
                       };

            var pic = Call("NasaAPI", "getPictureOfTheDay", args.ToArray());

        }

        private static void TestPack()
        {
            var apiKey = new DataParameter("apiKey", "*****");
            var str = new DataParameter("string", "מבחן");
            var targetLanguage = new DataParameter("targetLanguage", "en");
            var sourceLanguage = new DataParameter("sourceLanguage");

            var res = Call("GoogleTranslate", "translateAutomatic", apiKey, str, targetLanguage, sourceLanguage);
        }


        private static void TestPackWithImg(bool fromFile)
        {
            var subscriptionKey = new DataParameter("subscriptionKey", "*****");
            var image = fromFile ? new FileParameter("image", "dog.jpg") : (Parameter)new DataParameter("image", "http://cdn.litlepups.net/2015/08/31/cute-dog-baby-wallpaper-hd-21.jpg");
            var width = new DataParameter("width", "50");
            var height = new DataParameter("height", "50");
            var smartCropping = new DataParameter("smartCropping");

            var analyze = Call("MicrosoftComputerVision", "analyzeImage", subscriptionKey, image, width, height, smartCropping);

        }

        private static void TestPackWithImgFromStream()
        {
            var client = new HttpClient();
            var res = client.GetAsync("http://cdn.litlepups.net/2015/08/31/cute-dog-baby-wallpaper-hd-21.jpg").Result;
            var stream = res.Content.ReadAsStreamAsync().Result;

            var args = new List<Parameter>
                       {
                           new DataParameter("subscriptionKey", "*****"),
                           new FileParameter("image", stream, "nasaImage"),
                           new DataParameter("width", "50"),
                           new DataParameter("height", "50"),
                           new DataParameter("smartCropping")
                       };

            var analyze = Call("MicrosoftComputerVision", "analyzeImage", args.ToArray());

        }

        #endregion Tests

        static void Main(string[] args)
        {
            TestPublicPack();
            Console.ReadKey();
            TestPackWithImg(false);
            Console.ReadKey();
            TestPackWithImg(true);
            Console.ReadKey();
            TestPackWithImgFromStream();
            Console.ReadKey();
            TestPack();
            Console.ReadKey();
        }

        #region Response Classes

        private class NasaPicOfDay
        {
            public string Copyright { get; set; }

            public DateTime Date { get; set; }

            public string Explanation { get; set; }

            public string HdUrl { get; set; }

            public string Url { get; set; }

            [JsonProperty("media_type")]
            public string MediaType { get; set; }

            public string Title { get; set; }
        }

        private class AnalyzeImage
        {
            public List<Category> Categories { get; set; }

            public Dictionary<string, object> Metadata { get; set; }

            public string RequestId { get; set; }

            public class Category
            {
                public string Name { get; set; }

                public double Score { get; set; }

                #region Overrides of Object

                public override string ToString()
                {
                    return $"{Name} : {Score}";
                }

                #endregion
            }
        }

        #endregion

    }
}
