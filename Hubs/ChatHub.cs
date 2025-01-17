﻿using ChatroomService.DataService;
using ChatroomService.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatroomService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly SharedDb _shared;

        public ChatHub(SharedDb shared)
        {
            _shared = shared;
        }

        public async Task JoinChat(UserConnection conn)
        {
            await Clients.All
                .SendAsync("RecieveMessage", "admin, ", $"{conn.Username} has joined the chatroom"); 
        }

        public async Task JoinSpecificChatRoom(UserConnection conn)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.ChatRoom); 

            _shared.connections[Context.ConnectionId] = conn;   

            await Clients.Group(conn.ChatRoom)
                .SendAsync("JoinSpecificChatRoom", "admin", $"{conn.Username} has joined {conn.ChatRoom}");   
        }
        
        public async Task SendMessage(string msg)
        {
            if(_shared.connections.TryGetValue(Context.ConnectionId, out UserConnection conn))
            {
                await Clients.Group(conn.ChatRoom)
                    .SendAsync("RecieveSpecificMessage", conn.Username, msg);
            }
        }
    }
}



