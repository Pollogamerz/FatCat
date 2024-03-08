using System.Collections.Generic;
using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using TMPro;
using UnityEngine;

public class PanelChat : MonoBehaviour
{
    [SerializeField] private TMP_InputField _chatInput;
    [SerializeField] private TextMeshProUGUI _globalChatText;

    private List<string> _chatHistory = new List<string>();

    private void OnEnable()
    {
        // We subscribe for when a message of type 'BroadcastMessage', execute 'OnBroadcastMessage'
        InstanceFinder.ClientManager.RegisterBroadcast<MessageBroadcast>(OnMessageBroadcast);
        InstanceFinder.ServerManager.RegisterBroadcast<MessageBroadcast>(OnMessageBroadcastServer);
    }

    private void OnDisable()
    {
        InstanceFinder.ClientManager.UnregisterBroadcast<MessageBroadcast>(OnMessageBroadcast);
        InstanceFinder.ServerManager.UnregisterBroadcast<MessageBroadcast>(OnMessageBroadcastServer);
    }

    private void OnMessageBroadcast(MessageBroadcast messageBroadcast)
    {
        string message = messageBroadcast.user + ": " + messageBroadcast.message;
        _chatHistory.Add(message);
        if (_chatHistory.Count >= 10)
        {
            _chatHistory.RemoveAt(0);
        }
        PrintChatHistory();
    }

    private void OnMessageBroadcastServer(NetworkConnection connection, MessageBroadcast messageBroadcast)
    {
        messageBroadcast.user = connection.ClientId.ToString();
        InstanceFinder.ServerManager.Broadcast(messageBroadcast); // We send this message to ALL clients
                                                                  //InstanceFinder.ServerManager.Broadcast(connection, messageBroadcast); // Can use connection to send the message only to the user who sent this message
    }

    public void SendMessage()
    {
        string message = _chatInput.text;
        if (string.IsNullOrEmpty(message))
            return;

        _chatInput.text = "";

        MessageBroadcast messageBroadcast = new MessageBroadcast() // We generate the packet to be sent through the network
        {
            message = message
        };

        InstanceFinder.ClientManager.Broadcast(messageBroadcast); // Sends this packet to the server
    }

    public void PrintChatHistory()
    {
        _globalChatText.text = "";
        for (int i = 0; i < _chatHistory.Count; i++)
        {
            _globalChatText.text += _chatHistory[i];
            _globalChatText.text += '\n';
        }
    }

    public struct MessageBroadcast : IBroadcast
    {
        public string user;
        public string message;
    }
}
