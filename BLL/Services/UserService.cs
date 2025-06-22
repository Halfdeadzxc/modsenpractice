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

        public async Task<UserProfileDTO> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
            return _mapper.Map<UserProfileDTO>(user);
        }

        public async Task UpdateUserProfileAsync(Guid userId, UserUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            _mapper.Map(dto, user);
            await _userRepo.UpdateAsync(user, cancellationToken);
        }
    }

}