using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Services;
using DAL.Interfaces;
using DAL.Models;
using Moq;
using Xunit;

namespace Tests.BLLTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly IUserService _service;

        public UserServiceTests()
        {
            _service = new UserService(_userRepo.Object, _mapper.Object);
        }

        [Fact]
        public async Task GetUserProfileAsync_ReturnsMappedDto()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Username = "u" };
            var dto = new UserProfileDTO { Id = userId, Username = "u" };

            _userRepo
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mapper
                .Setup(m => m.Map<UserProfileDTO>(user))
                .Returns(dto);

            var result = await _service.GetUserProfileAsync(userId);

            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_UserNotFound_Throws()
        {
            var userId = Guid.NewGuid();
            var update = new UserUpdateDTO { Bio = "bio" };

            _userRepo
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.UpdateUserProfileAsync(userId, update));
        }

        [Fact]
        public async Task UpdateUserProfileAsync_Valid_MapsAndUpdates()
        {
            var userId = Guid.NewGuid();
            var update = new UserUpdateDTO { Bio = "newbio" };
            var user = new User { Id = userId, Bio = "old" };

            _userRepo
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mapper
                .Setup(m => m.Map(update, user))
                .Returns(user);

            _userRepo
                .Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            await _service.UpdateUserProfileAsync(userId, update);

            _mapper.Verify(m => m.Map(update, user), Times.Once);
            _userRepo.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
