using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{   
   
    public class AccountController : BaseApiController
    {   
        
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;
        }
        
        [HttpPost("register")]

        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {   
            
            if (await EmailExists(registerDto.email)) return BadRequest("Email is taken");

            
            
            var user = _mapper.Map<AppUser>(registerDto);
            
            user.userFirstName = registerDto.userFirstName;
            user.userLastName = registerDto.userLastName;
            user.email = registerDto.email;
             
             
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            
            return new UserDto
            {
                userFirstName=user.userFirstName,

                userLastName= user.userLastName,

                email = user.email,

                
                Token = _tokenService.CreateToken(user)
            
            };

            

        }
        
        
        [HttpPost("login")]

        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {   
            
            var user = await _context.Users.SingleOrDefaultAsync(
                x => x.email == loginDto.email);

            if (user == null) return Unauthorized("Invalid Email");
            
            
            return new UserDto
            {
                userFirstName=user.userFirstName,

                userLastName= user.userLastName,

                email = user.email,


               
                Token = _tokenService.CreateToken(user)
            };
        }
        
       
        private async Task<bool> EmailExists(string email)
        {
            return await _context.Users.AnyAsync(x => x.email == email);
        }

        
    }
}