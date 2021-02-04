using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;



namespace API.Interfaces
{
    public interface IUserRepository
    {
        

        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetUsersAsync();

        
        
        Task<AppUser> GetUserByEmailAsync(string email);

        Task<IEnumerable<MemberDto>> GetMembersAsync();

        Task<MemberDto> GetMemberAsync(string email);

    }
}