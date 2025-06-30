using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Services;
using DAL.Interfaces;
using DAL.Models;
using Moq;
using Xunit;

namespace Tests.BLLTests
{
    public class FeedServiceTests
    {
        private readonly Mock<IFeedRepository> _feedRepo = new();
        private readonly Mock<ILikeRepository> _likeRepo = new();
        private readonly Mock<IBookmarkRepository> _bookmarkRepo = new();
        private readonly Mock<IRepostRepository> _repostRepo = new();
        private readonly Mock<ISubscriptionRepository> _subscriptionRepo = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly FeedService _service;
        private readonly CancellationToken _ct = CancellationToken.None;

        public FeedServiceTests()
        {
            _service = new FeedService(
                _feedRepo.Object,
                _likeRepo.Object,
                _bookmarkRepo.Object,
                _repostRepo.Object,
                _subscriptionRepo.Object,
                _mapper.Object
            );
        }

        [Fact]
        public async Task GetFeedAsync_Recent_ReturnsMappedPosts()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var posts = new List<Post> { new() { Id = postId } };
            var dtos = new List<PostDTO> { new() { Id = postId } };

            _feedRepo
                .Setup(r => r.GetRecentPostsAsync(1, 10, _ct))
                .ReturnsAsync(posts);

            _mapper
                .Setup(m => m.Map<List<PostDTO>>(posts))
                .Returns(dtos);

            _likeRepo
                .Setup(r => r.ExistsAsync(userId, postId, _ct))
                .ReturnsAsync(true);
            _bookmarkRepo
                .Setup(r => r.ExistsAsync(userId, postId, _ct))
                .ReturnsAsync(false);
            _repostRepo
                .Setup(r => r.ExistsAsync(userId, postId, _ct))
                .ReturnsAsync(true);

            var result = (await _service.GetFeedAsync(userId, 1, 10, "recent", _ct)).ToList();

            Assert.Single(result);
            Assert.True(result[0].IsLiked);
            Assert.False(result[0].IsBookmarked);
            Assert.True(result[0].IsReposted);
        }

        [Fact]
        public async Task GetFeedAsync_Popular_ReturnsMappedPosts()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var posts = new List<Post> { new() { Id = postId } };
            var dtos = new List<PostDTO> { new() { Id = postId } };

            _feedRepo
                .Setup(r => r.GetPopularPostsAsync(It.IsAny<DateTime>(), 1, 10, _ct))
                .ReturnsAsync(posts);

            _mapper
                .Setup(m => m.Map<List<PostDTO>>(posts))
                .Returns(dtos);

            _likeRepo
                .Setup(r => r.ExistsAsync(userId, postId, _ct))
                .ReturnsAsync(false);
            _bookmarkRepo
                .Setup(r => r.ExistsAsync(userId, postId, _ct))
                .ReturnsAsync(true);
            _repostRepo
                .Setup(r => r.ExistsAsync(userId, postId, _ct))
                .ReturnsAsync(false);

            var result = (await _service.GetFeedAsync(userId, 1, 10, "popular", _ct)).ToList();

            Assert.Single(result);
            Assert.False(result[0].IsLiked);
            Assert.True(result[0].IsBookmarked);
            Assert.False(result[0].IsReposted);
        }

        [Fact]
        public async Task GetFeedAsync_Subscriptions_ReturnsAuthorPosts()
        {
            var userId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            _subscriptionRepo
                .Setup(r => r.GetFollowingAsync(userId, _ct))
                .ReturnsAsync(new List<User> { new() { Id = authorId } });

            var posts = new List<Post> { new() { Id = postId, AuthorId = authorId } };
            var dtos = new List<PostDTO> { new() { Id = postId } };

            _feedRepo
                .Setup(r => r.GetPostsByAuthorsAsync(
                    It.Is<IEnumerable<Guid>>(ids => ids.SequenceEqual(new[] { authorId })),
                    1,
                    10,
                    _ct))
                .ReturnsAsync(posts);

            _mapper
                .Setup(m => m.Map<List<PostDTO>>(posts))
                .Returns(dtos);

            _likeRepo
                .Setup(r => r.ExistsAsync(userId, postId, _ct))
                .ReturnsAsync(true);
            _bookmarkRepo
                .Setup(r => r.ExistsAsync(userId, postId, _ct))
                .ReturnsAsync(true);
            _repostRepo
                .Setup(r => r.ExistsAsync(userId, postId, _ct))
                .ReturnsAsync(true);

            var result = (await _service.GetFeedAsync(userId, 1, 10, "subscriptions", _ct)).ToList();

            Assert.Single(result);
            Assert.True(result[0].IsLiked);
            Assert.True(result[0].IsBookmarked);
            Assert.True(result[0].IsReposted);
        }
    }
}
