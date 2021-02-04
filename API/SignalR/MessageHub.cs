using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{   
    [Authorize]

    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<PrescenceHub> _prescenceHub;
        private readonly PrescenceTracker _tracker;

        public MessageHub(IMessageRepository messageRepository,
         IMapper mapper, IUserRepository userRepository,
         IHubContext<PrescenceHub> prescenceHub, PrescenceTracker tracker)
        {
            _tracker = tracker;
            _prescenceHub = prescenceHub;
            _userRepository = userRepository;
            _mapper = mapper;
            _messageRepository = messageRepository;

        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUseremail(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(Context, groupName);
            var messages = await _messageRepository.
            GetMessagesThread(Context.User.GetUseremail(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var useremail = Context.User.GetUseremail();


            if (useremail == createMessageDto.Recipientemail)
                throw new HubException("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByEmailAsync(useremail);
            var recipient = await _userRepository.GetUserByEmailAsync(createMessageDto.Recipientemail);

            if (recipient == null) throw new HubException("Not found User");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderName = sender.userFirstName,
                RecipientName = recipient.userFirstName,
                SenderEmail = sender.email,
                RecipientEmail = recipient.email,
                Content = createMessageDto.Content
            };



            var groupName = GetGroupName(sender.email, recipient.email);

            var group = await _messageRepository.GetMessageGroup(groupName);

            if (group.Connections.Any(x => x.Useremail == recipient.email))
            {

            }
            else
            {
                var connections = await _tracker.GetConnectionsForUser(recipient.email);
               if(connections !=null)
               {
                    await _prescenceHub.Clients.Clients(connections).SendAsync
                    (
                        "NewMessageReceived",
                        new {username = sender.userFirstName,useremail =sender.email}
                    );
                }
            }

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {

                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }


        }
        private async Task<bool> AddToGroup(HubCallerContext context, string groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);

            var connection = new Connection(Context.ConnectionId, Context.User.GetUseremail());

            if (group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            return await _messageRepository.SaveAllAsync();


        }

        private async Task RemoveFromMessageGroup(string connectionId)
        {
            var connection = await _messageRepository.GetConnection(connectionId);

            _messageRepository.RemoveConnection(connection);

            await _messageRepository.SaveAllAsync();
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }


    }
}