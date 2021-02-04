using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Linq;


namespace API.Controllers
{   
    
    [Authorize]
    public class MessagesController : BaseApiController
    {    
        
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
       
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository,
        
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
          
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }
    
    
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {   
       
        var useremail =  User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
        
        var sender = await _userRepository.GetUserByEmailAsync(useremail);
        var recipient = await _userRepository.GetUserByEmailAsync(createMessageDto.Recipientemail);

        if (recipient == null) return NotFound();
        
       
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
        
        
        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message");

    }
    
    
    [HttpGet]

    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]
    MessageParams messageParams)
    {   
        
        messageParams.email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        
        var messages = await _messageRepository.GetMessagesForUser(messageParams);
        
      
        Response.AddPaginationHeader(messages.CurentPage,messages.PageSize,messages.TotalCount,
        messages.TotalPages);

        return messages ;
    }

    
    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetAll(int id)
    {
        
      
        var message = await _messageRepository.GetMessage(id);

        

        return message ;
    }

    
    [HttpGet("thread/{useremail}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string useremail)
    {   
        
        var currentUseremail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
       
        return Ok(await _messageRepository.GetMessagesThread(currentUseremail,useremail));
          

    }
   
    [HttpDelete("{id}")]

    public async Task<ActionResult> DeleteMessages(int id)
    {
        
      

        var message = await _messageRepository.GetMessage(id);

        
        
        _messageRepository.DeleteMessage(message);

        if(await _messageRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting message");

    }

}
} 