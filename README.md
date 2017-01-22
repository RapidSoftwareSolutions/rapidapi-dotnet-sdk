<p align="center">
  <img src="https://storage.googleapis.com/rapid_connect_static/static/github-header.png" width=350 />
</p>

## Overview
RapidAPI is the world's first opensource API marketplace. It allows developers to discover and connect to the world's top APIs more easily and manage multiple API connections in one place.

## .Net SDK

First of all, install the sdk using NuGet.
There are two ways for doing that.The first one is using the NuGet Package Manager console,
and the other is using the VIsual Studio Extension.

### NuGet Package Manager Console

run the following command in the console to install the sdk and its dependencies:

    PM> Install-Package RapidAPISDK
    
### NuGet Visual Studio Extension

Follow the steps bellow:

1. In the **Solution Explorer**, right-click your project and then click on **Manage NuGet Packages for Solution...**

2. Enter the **Browse** tag and then in the search box, type "RapidAPISDK" and hit enter.

3. After the search completes, find the sdk, check the righ box and click install.


## Initialization
Import RapidAPISDK by putting the following code in the head of your file
```cs
#using RapidAPISDK;
```
  
Now initialize it using:
```cs
private static RapidAPI RapidApi = new RapidAPI("PROJECT", "TOKEN");
```
  
## Usage

First of all, we will prepare the body, we will use Array which You can add as many arguments as you wish due to the API requirements. 
In this example we generated a list and send it as an array.
```cs
var body = new List<Parameter>()
{
   new DataParameter("parameterName1", "value1"),
   new FileParameter("parameterName2", "value2"),
   new DataParameter("parameterName3", "value3")
};
 ```
 Note: You have two kinds of parameters you can send - Data or File. A File parameter can be a file stream or a file path
 and Data parameter is any other parameter.
 
To use any block in the marketplace, just copy it's code snippet and paste it in your code. For example, the following is    the snippet for the **NasaAPI.getPictureOfTheDay** block:
 ```cs
try
{
    var res = RapidApi.Call("NasaAPI", "getPictureOfTheDay", body.ToArray()).Result;
    object payload;
    if (res.TryGetValue("success", out payload)){
        Console.WriteLine("success: " + payload);
    else
    {
        res.TryGetValue("error", out payload);
        Console.WriteLine("error: " + payload);
    }
}
catch (RapidAPIServerException e)
{
    Console.WriteLine("Server error: " + e);
}
catch (Exception e)
{
    Console.WriteLine("Unknown exeption: " + e);
}
 ```

  
**Notice** that the `error` event will also be called if you make an invalid block call (for example - the package you refer to does not exist).

## Using Files
Whenever a block in RapidAPI requires a file, you can either pass a URL to the file, a path to the file or a read stream.


#### URL
The following code will call the block MicrosoftComputerVision.analyzeImage with a URL of an image:
```cs
var subscriptionKey = new DataParameter("subscriptionKey", "********");
var image = new DataParameter("image", "http://cdn.litlepups.net/2015/08/31/cute-dog-baby-wallpaper-hd-21.jpg");
var width = new DataParameter("width", "50");
var height = new DataParameter("height", "50");
var smartCropping = new DataParameter("smartCropping");

var analyze = Call("MicrosoftComputerVision", "analyzeImage", subscriptionKey, image, width, height, smartCropping);
```
#### Post File
If the file is locally stored, you can just use this line instead:
```cs
var image = new FileParameter("image", "IMAGE_PATH");  
```
Or you can post a file as a stream:
```cs
var image =  new FileParameter("image", stream, "IMAGE_NAME");
```

##Issues:

As this is a pre-release version of the SDK, you may expirience bugs. Please report them in the issues section to let us know. You may use the intercom chat on rapidapi.com for support at any time.

##License:

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
