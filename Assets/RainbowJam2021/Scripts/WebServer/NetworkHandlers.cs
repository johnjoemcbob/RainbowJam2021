using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Networking;

public interface IWebRequestHandler
{
	void HandleRequest( string result );
}

public class BlankRequestHandler : IWebRequestHandler
{
	public void HandleRequest( string result )
	{
		
	}
}

public class GetGhostPathsHandler : IWebRequestHandler
{
	[System.Serializable]
	public struct Response
	{
		public string[] all;
	}

	public void HandleRequest( string result )
	{
		var resp = JsonUtility.FromJson<Response>( result );

		GameObject.FindObjectOfType<GhostDownloader>().InstantiateGhosts( resp.all );
	}
}

public class PostGhostPathHandler : IWebRequestHandler
{
	public void HandleRequest( string result )
	{
		// No call back currently
	}
}
