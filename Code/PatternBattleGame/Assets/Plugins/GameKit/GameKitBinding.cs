using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


#if UNITY_IPHONE
// enum to match up with GKSessionMode in native code.  Allows forcing a device to be client, server or both (Peer)
public enum GameKitSessionMode
{
	Server = 0,
	Client,
	Peer
};


// All Objective-C exposed methods should be bound here
public class GameKitBinding
{
    [DllImport("__Internal")]
    private static extern void _gameKitShowPeerPicker( bool includeOnlineChooser );

	// Shows the peer picker enabling users to connect to other iDevices over Bluetooth or optionally Online (WiFi)
    public static void showPeerPicker( bool includeOnlineChooser )
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameKitShowPeerPicker( includeOnlineChooser );
    }
	

    [DllImport("__Internal")]
    private static extern void _gameKitInitializeSessionForOnlineOnly( string sessionId, string displayName );

	// CUSTOM LOBBY: Initializes a game session for online (WiFi) play with no picker optionally with a sessionId and displayName.
	// The sessionId is used to identify your game. Each device will be able to connect only to other
	// devices with the same sessionId.
    public static void initializeSessionForOnlineOnly()
    {
        initializeSessionForOnlineOnly( null, null );
    }
	
	public static void initializeSessionForOnlineOnly( string sessionId, string displayName )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameKitInitializeSessionForOnlineOnly( sessionId, displayName );
	}
	


    [DllImport("__Internal")]
    private static extern void _gameKitSetAutoConnectPeers( bool shouldAutoConnect );

	// By setting this to false and using an online (WiFi) session, you will be required to manually
	// accept all connections.  This is an advanced option and only required if you want full control
	// over the entire lobby and connection system.
    public static void setAutoConnectPeers( bool shouldAutoConnect )
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameKitSetAutoConnectPeers( shouldAutoConnect );
    }
	
	
    [DllImport("__Internal")]
    private static extern string _gameKitGetConnectedPeers();

	// Gets a list of the connected peers from native
    public static string[] getConnectedPeers()
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameKitGetConnectedPeers().Split( ',' );
		return new string[0];
    }


    [DllImport("__Internal")]
    private static extern string _gameKitSendMessage( bool reliably, string gameObject, string method, string param );

	// Calls the given method with parameter on the remote gameObject
    public static string sendData( string gameObject, string method, string param, bool reliably )
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameKitSendMessage( reliably, gameObject, method, param );
		return string.Empty;
    }


    [DllImport("__Internal")]
    private static extern string _gameKitSendMessageToPeers( bool reliably, string peerIds, string gameObject, string method, string param );

	// Calls the given method with parameter on the remote gameObject and sends it only to the peers specified
    public static string sendMessageToPeers( string[] peerIds, string gameObject, string method, string param, bool reliably )
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameKitSendMessageToPeers( reliably, string.Join( ",", peerIds ), gameObject, method, param );
		return string.Empty;
    }
	
	
	[DllImport("__Internal")]
	private static extern string _gameKitSendRawMessageToAllPeers( string gameObject, string method, byte[] param, int length, bool reliably );

	// Sends a raw byte array message to all connected devices forwarding through to the specified remote gameObject and method.
	public static string sendRawMessageToAllPeers( string gameObject, string method, byte[] param, bool reliably )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameKitSendRawMessageToAllPeers( gameObject, method, param, param.Length, reliably );
		return string.Empty;
    }


	[DllImport("__Internal")]
	private static extern string _gameKitSendRawMessageToPeers( string playerIds, string gameObject, string method, byte[] param, int length, bool reliably );

	// Sends a raw byte array message to the specified players forwarding through to the specified remote gameObject and method.
	public static string sendRawMessageToPeers( string[] playerIds, string gameObject, string method, byte[] param, bool reliably )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameKitSendRawMessageToPeers( string.Join( ",", playerIds ), gameObject, method, param, param.Length, reliably );
		return string.Empty;
    }
	
	
    [DllImport("__Internal")]
    private static extern void _gameKitSetSessionMode( int sessionMode );

	// Defaults to Peer.  You can choose to force a client to be client or server by setting this.
	// Note: this only has an effect for online (WiFi) sessions.  Nearby (Bluetooth) sessions
	// are always requied to be in Peer mode
    public static void setSessionMode( GameKitSessionMode sessionMode )
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameKitSetSessionMode( (int)sessionMode );
    }


    [DllImport("__Internal")]
    private static extern int _gameKitCurrentSessionMode();

	// Gets the current GameKitSessionMode state
    public static GameKitSessionMode currentSessionMode()
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
		{
			int mode = _gameKitCurrentSessionMode();
			GameKitSessionMode sessionMode = (GameKitSessionMode)System.Enum.ToObject( typeof( GameKitSessionMode ), mode );
			return sessionMode;
		}
		
		return GameKitSessionMode.Server;
    }


    [DllImport("__Internal")]
    private static extern string _gameKitDisplayNameForPeer( string peerId );

	// Gets the display name of the given peerId
    public static string displayNameForPeer( string peerId )
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameKitDisplayNameForPeer( peerId );
		return string.Empty;
    }


    [DllImport("__Internal")]
    private static extern string _gameKitGetPeerId();

	// Gets the peerId of the current device
    public static string getPeerId()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameKitGetPeerId();
		return string.Empty;
    }


    [DllImport("__Internal")]
    private static extern void _gameKitInvalidateSession();

	// Ends the current GameKit session and performs clean up
    public static void invalidateSession()
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameKitInvalidateSession();
		
		// clear out the peers
		GameKitManager.connectedPeers.Clear();
		GameKitManager.availablePeers.Clear();
    }
	
	
    [DllImport("__Internal")]
    private static extern void _gameKitConnectToPeer( string peerId );

	// Attempts to connect to the given peerId.  Required for custom lobby implementations.
    public static void connectToPeer( string peerId )
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameKitConnectToPeer( peerId );
    }
	
	
    [DllImport("__Internal")]
    private static extern void _gameKitAcceptConnectionFromPeer( string peerId );

	// Accepts a connection from the given peer.  Required for custom lobby implementations.
    public static void acceptConnectionFromPeer( string peerId )
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameKitAcceptConnectionFromPeer( peerId );
    }
	
	
    [DllImport("__Internal")]
    private static extern void _gameKitSetVoiceChatEnabled( bool voiceChatEnabled );

	// This must be set to true before opening the peer picker if you intend to use voice chat!
    public static void setVoiceChatEnabled( bool voiceChatEnabled )
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameKitSetVoiceChatEnabled( voiceChatEnabled );
    }

	
    [DllImport("__Internal")]
    private static extern float _gameKitGetRemoteParticipantVolume();

	// Gets the volume level of the incoming voice chat
    public static float getRemoteParticipantVolume()
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameKitGetRemoteParticipantVolume();
		
		return 0;
    }
	
	
    [DllImport("__Internal")]
    private static extern void _gameKitSetRemoteParticipantVolume( float volume );

	// Sets the volume level of the incoming voice chat
    public static void setRemoteParticipantVolume( float volume )
    {
        // Call plugin only when running on real device
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameKitSetRemoteParticipantVolume( volume );
    }


	[DllImport("__Internal")]
	private static extern void _gameKitEnableInputMetering( bool shouldEnable );

	// Enables metering of the input stream. This must be called before getInputMeterLevel.
	public static void enableInputMetering( bool shouldEnable )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameKitEnableInputMetering( shouldEnable );
	}


	[DllImport("__Internal")]
	private static extern void _gameKitEnableOutputMetering( bool shouldEnable );

	// Enables metering of the output stream. This must be called before getOutputMeterLevel.
	public static void enableOutputMetering( bool shouldEnable )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameKitEnableOutputMetering( shouldEnable );
	}


	[DllImport("__Internal")]
	private static extern float _gameKitGetInputMeterLevel();

	// Gets the current meter level of the input audio stream
	public static float getInputMeterLevel()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameKitGetInputMeterLevel();

		return 0;
	}


	[DllImport("__Internal")]
	private static extern float _gameKitGetOutputMeterLevel();

	// Gets the current meter level of the output audio stream
	public static float getOutputMeterLevel()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameKitGetOutputMeterLevel();

		return 0;
	}

}
#endif
