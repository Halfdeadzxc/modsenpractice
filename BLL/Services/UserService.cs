using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<UserProfileDTO> GetUserProfileAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            return _mapper.Map<UserProfileDTO>(user);
        }

        public async Task UpdateUserProfileAsync(Guid userId, UserUpdateDTO dto)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found");

            _mapper.Map(dto, user);
            await _userRepo.UpdateAsync(user);
        }
    }
}