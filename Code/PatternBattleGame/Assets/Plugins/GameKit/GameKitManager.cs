using UnityEngine;
using System;
using System.Collections.Generic;
using Prime31;


#if UNITY_IPHONE
public class GameKitManager : AbstractManager
{
	// Fired when a user cancels the peer picker
	public static event Action peerPickerCancelledEvent;
	
	// Fired whenever a peer successfully connects via Bluetooth
	public static event Action<string> peerConnectedEvent;
	
	// Fired whenever a peer disconnects
	public static event Action<string> peerDisconnectedEvent;
	
	// Manual picker delegates and events
	// Fired when a peer becomes available either via WiFi or Bluetooth
	public static event Action<string> peerBecameAvailableEvent;
	
	// Fired when a peer becomes unavailable
	public static event Action<string> peerBecameUnavailableEvent;
	
	// Fired when a connection invitation is sent by a remote peer
	public static event Action<string> receivedConnectionRequestFromPeerEvent;
	
	// Fired when an attempt to acceptConnectionFromPeer fails
	public static event Action<string> acceptConnectionWithPeerFailed;
	
	// Voice chat specific events
	// Fired when a peer connects for voice chat
	public static event Action<string> peerConnectedForVoiceChatEvent;
	
	// Fired when a peer disconnects from voice chat
	public static event Action<string> peerDisconnectedFromVoiceChatEvent;
	
	
	// Local storage of available and connected peers for quick access
	public static List<string> connectedPeers = new List<string>();
	public static List<string> availablePeers = new List<string>();
	
	
    static GameKitManager()
    {
		AbstractManager.initialize( typeof( GameKitManager ) );
    }
	
	
	public static bool isConnected()
	{
		return ( connectedPeers.Count > 0 );
	}
	
	
	public void didConnectToPeer( string peerId )
	{
		// save the peerId locally as well for easy reference
		connectedPeers.Add( peerId );
		
		// kick off the event
		if( peerConnectedEvent != null )
			peerConnectedEvent( peerId );
	}
	
	
	public void peerPickerDidCancel( string emptyString )
	{
		connectedPeers.Clear();
		
		if( peerPickerCancelledEvent != null )
			peerPickerCancelledEvent();
	}
	
	
	public void peerDidDisconnect( string peerId )
	{
		// remove the peer from the local list
		connectedPeers.Remove( peerId );
		
		// kick off the event
		if( peerDisconnectedEvent != null )
			peerDisconnectedEvent( peerId );
	}
	
	
	#region Manual Interface callbacks
	
	public void peerDidBecameAvailable( string peerId )
	{
		// Keep the availablePeers list in sync
		availablePeers.Add( peerId );
		
		if( peerBecameAvailableEvent != null )
			peerBecameAvailableEvent( peerId );
	}
	
	
	public void peerDidBecameUnavailable( string peerId )
	{
		// Keep the availablePeers list in sync
		availablePeers.Remove( peerId );
		
		if( peerBecameUnavailableEvent != null )
			peerBecameUnavailableEvent( peerId );
	}
	
	
	public void didReceiveConnectionRequestFromPeer( string peerId )
	{
		if( receivedConnectionRequestFromPeerEvent != null )
			receivedConnectionRequestFromPeerEvent( peerId );
	}
	
	
	public void acceptConnectionWithPeerDidFail( string peerId )
	{
		if( acceptConnectionWithPeerFailed != null )
			acceptConnectionWithPeerFailed( peerId );
	}
	
	#endregion;
	
	
	#region Voice Chat callbacks
	
	public void voiceChatServiceDidStart( string peerId )
	{
		// kick off the event
		if( peerConnectedForVoiceChatEvent != null )
			peerConnectedForVoiceChatEvent( peerId );
	}
	
	
	public void voiceChatServiceDidStop( string peerId )
	{
		// kick off the event
		if( peerDisconnectedFromVoiceChatEvent != null )
			peerDisconnectedFromVoiceChatEvent( peerId );
	}
	
	#endregion;
}
#endif