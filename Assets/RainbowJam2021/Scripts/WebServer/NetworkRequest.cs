using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class NetworkRequest : MonoBehaviour
{
	public static NetworkRequest Instance;

	// Server URL
#if INTERNAL
	public static readonly string SV_ROOT = "http://rainbowjam2021/";
#else
	public static readonly string SV_ROOT = "https://johnjoemcbob.com/rainbowjam2021/";
#endif

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;

#if INTERNAL
		Debug.Log( "Warning: Internal server mode active!" );
		Debug.Log( "Warning: Internal server mode active!" );
		Debug.Log( "Warning: Internal server mode active!" );
		Debug.Log( "Warning: Internal server mode active!" );
#endif
	}
	#endregion

	#region DoNetworkRequest
	private static IEnumerator DoNetworkRequest( string url, IWebRequestHandler handler, WWWForm form = null, Dictionary<string, string> headers = null )
	{
		UnityWebRequest www;
			if ( form == null )
			{
				www = UnityWebRequest.Get( url );
			}
			else
			{
				www = UnityWebRequest.Post( url, form ); www.useHttpContinue = false;
			}
		if ( headers != null )
		{
			foreach ( KeyValuePair<string, string> header in headers )
			{
				www.SetRequestHeader( header.Key, header.Value );
			}
		}

		// Wait for response
		yield return www.SendWebRequest();

		// Handle response
		if ( www.isNetworkError || www.isHttpError )
		{
			Debug.Log( url );
			Debug.Log( www.error );
			if ( www.responseCode == 404 )
			{
				handler.HandleRequest( "404: " + www.downloadHandler.text );
			}
			else
			{
				Debug.Log( "There seems to be a network error. Try again later." );
			}
		}
		else
		{
			//if ( true ) // TODO TEMP REMOVE
			//{
			//	Debug.Log( url );
			//	Debug.Log( www.downloadHandler.text );
			//}
			handler.HandleRequest( www.downloadHandler.text );
		}
	}
	#endregion

	#region Specific Cases
	public void GetGhostPaths( IWebRequestHandler handler )
	{
		string script = SV_ROOT + "getGhostPaths.php";
		Instance.StartCoroutine( DoNetworkRequest( $"{script}", handler ) );
	}

	public void PostGhostPath( string json, IWebRequestHandler handler )
	{
		string script = SV_ROOT + "postGhostPath.php";

		WWWForm form = new WWWForm();
			form.AddField( "json", json );
		Instance.StartCoroutine( DoNetworkRequest( $"{script}", handler, form ) );
	}
	#endregion
}
