using System;
namespace RapidAPISDK.Events
{
	public delegate void MessageCallback(string message);
	public delegate void ErrorCallback(string reason);
	public delegate void TimeOutCallback();
	public delegate void ConnectCallback();
}
