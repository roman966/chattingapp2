using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{   
    
    [Authorize]

     
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;

        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;


        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {   
            
            var users = await _userRepository.GetMembersAsync();

            return Ok(users);
        }
        
       
        [HttpGet("{email}")]
        public async Task<ActionResult<MemberDto>> GetUser(string email)
        {    
            
            var user = await _userRepository.GetMemberAsync(email);
            
            return Ok(user);

        }

        

        





    }
}