using AutoMapper;
using BLL.DTO;
using BLL.Services;
using DAL.Interfaces;
using DAL.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.BLLTests
{
    public class InteractionServiceTests
    {
        private readonly Mock<IBookmarkRepository> _bookmarkRepoMock;
        private readonly Mock<ICommentRepository> _commentRepoMock;
        private readonly Mock<ILikeRepository> _likeRepoMock;
        private readonly Mock<IRepostRepository> _repostRepoMock;
        private readonly Mock<ISubscriptionRepository> _subscriptionRepoMock;
        private readonly Mock<IPostRepository> _postRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly InteractionService _service;

        public InteractionServiceTests()
        {
            _bookmarkRepoMock = new Mock<IBookmarkRepository>();
            _commentRepoMock = new Mock<ICommentRepository>();
            _likeRepoMock = new Mock<ILikeRepository>();
            _repostRepoMock = new Mock<IRepostRepository>();
            _subscriptionRepoMock = new Mock<ISubscriptionRepository>();
            _postRepoMock = new Mock<IPostRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();

            _service = new InteractionService(
                _bookmarkRepoMock.Object,
                _commentRepoMock.Object,
                _likeRepoMock.Object,
                _repostRepoMock.Object,
                _subscriptionRepoMock.Object,
                _postRepoMock.Object,
                _userRepoMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task AddBookmark_WhenBookmarkDoesNotExist_ShouldAddBookmarkAndIncrementCount()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId, BookmarkCount = 0 };

            _bookmarkRepoMock.Setup(x => x.GetByIdAsync(userId, postId, default))
                .ReturnsAsync((Bookmark)null);

            _postRepoMock.Setup(x => x.GetByIdAsync(postId, default))
                .ReturnsAsync(post);

            await _service.AddBookmarkAsync(userId, postId);

            _bookmarkRepoMock.Verify(x => x.AddAsync(
                It.Is<Bookmark>(b => b.UserId == userId && b.PostId == postId),
                default
            ), Times.Once);

            _postRepoMock.Verify(x => x.UpdateAsync(
                It.Is<Post>(p => p.Id == postId && p.BookmarkCount == 1),
                default
            ), Times.Once);
        }

        [Fact]
        public async Task AddComment_ShouldAddCommentAndIncrementCount()
        {
            var authorId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var commentDto = new CommentCreateDTO
            {
                PostId = postId,
                Content = "Test comment"
            };

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                AuthorId = authorId,
                Content = "Test comment"
            };

            var post = new Post { Id = postId, CommentCount = 0 };
            var commentDtoResult = new CommentDTO { Id = comment.Id };

            _commentRepoMock.Setup(x => x.AddAsync(It.IsAny<Comment>(), default))
                .ReturnsAsync(comment);

            _postRepoMock.Setup(x => x.GetByIdAsync(postId, default))
                .ReturnsAsync(post);

            _mapperMock.Setup(x => x.Map<CommentDTO>(comment))
                .Returns(commentDtoResult);

            var result = await _service.AddCommentAsync(commentDto, authorId);

            Assert.Equal(comment.Id, result.Id);

            _commentRepoMock.Verify(x => x.AddAsync(
                It.Is<Comment>(c =>
                    c.PostId == postId &&
                    c.AuthorId == authorId &&
                    c.Content == "Test comment"),
                default
            ), Times.Once);

            _postRepoMock.Verify(x => x.UpdateAsync(
                It.Is<Post>(p => p.Id == postId && p.CommentCount == 1),
                default
            ), Times.Once);
        }

        [Fact]
        public async Task Subscribe_WhenSubscriptionDoesNotExist_ShouldAddSubscription()
        {
            var followerId = Guid.NewGuid();
            var followingId = Guid.NewGuid();

            _subscriptionRepoMock.Setup(x => x.GetByIdAsync(followerId, followingId, default))
                .ReturnsAsync((Subscription)null);

            await _service.SubscribeAsync(followerId, followingId);

            _subscriptionRepoMock.Verify(x => x.AddAsync(
                It.Is<Subscription>(s =>
                    s.FollowerId == followerId &&
                    s.FollowingId == followingId &&
                    s.Status == "approved"),
                default
            ), Times.Once);
        }

        [Fact]
        public async Task Subscribe_WhenSubscribingToSelf_ShouldThrowArgumentException()
        {
            var userId = Guid.NewGuid();

            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.SubscribeAsync(userId, userId)
            );
        }

        [Fact]
        public async Task DeleteComment_WhenAuthorized_ShouldDeleteCommentAndDecrementCount()
        {
            var commentId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = commentId,
                AuthorId = authorId,
                PostId = postId
            };

            var post = new Post { Id = postId, CommentCount = 1 };

            _commentRepoMock.Setup(x => x.GetByIdAsync(commentId, default))
                .ReturnsAsync(comment);

            _postRepoMock.Setup(x => x.GetByIdAsync(postId, default))
                .ReturnsAsync(post);

            await _service.DeleteCommentAsync(commentId, authorId);

            _commentRepoMock.Verify(x => x.DeleteAsync(commentId, default), Times.Once);
            _postRepoMock.Verify(x => x.UpdateAsync(
                It.Is<Post>(p => p.Id == postId && p.CommentCount == 0),
                default
            ), Times.Once);
        }

        [Fact]
        public async Task DeleteComment_WhenUnauthorized_ShouldThrowUnauthorizedAccessException()
        {
            var commentId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = commentId,
                AuthorId = authorId
            };

            _commentRepoMock.Setup(x => x.GetByIdAsync(commentId, default))
                .ReturnsAsync(comment);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.DeleteCommentAsync(commentId, differentUserId)
            );
        }

        [Fact]
        public async Task RemoveBookmark_ShouldRemoveBookmarkAndDecrementCount()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId, BookmarkCount = 1 };

            _postRepoMock.Setup(x => x.GetByIdAsync(postId, default))
                .ReturnsAsync(post);

            await _service.RemoveBookmarkAsync(userId, postId);

            _bookmarkRepoMock.Verify(x => x.DeleteAsync(userId, postId, default), Times.Once);
            _postRepoMock.Verify(x => x.UpdateAsync(
                It.Is<Post>(p => p.Id == postId && p.BookmarkCount == 0),
                default
            ), Times.Once);
        }

        [Fact]
        public async Task GetBookmarks_ShouldReturnMappedPosts()
        {
            var userId = Guid.NewGuid();
            var posts = new List<Post>
            {
                new Post { Id = Guid.NewGuid() },
                new Post { Id = Guid.NewGuid() }
            };
            var expectedDtos = posts.Select(p => new PostDTO { Id = p.Id }).ToList();

            _bookmarkRepoMock.Setup(x => x.GetBookmarksAsync(userId, default))
                .ReturnsAsync(posts);
            _mapperMock.Setup(x => x.Map<IEnumerable<PostDTO>>(posts))
                .Returns(expectedDtos);

            var result = await _service.GetBookmarksAsync(userId);

            Assert.Equal(expectedDtos.Count, result.Count());
            Assert.Equal(expectedDtos.First().Id, result.First().Id);
        }

        [Fact]
        public async Task AddLike_WhenLikeDoesNotExist_ShouldAddLikeAndIncrementCount()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId, LikeCount = 0 };

            _likeRepoMock.Setup(x => x.GetByIdAsync(userId, postId, default))
                .ReturnsAsync((Like)null);
            _postRepoMock.Setup(x => x.GetByIdAsync(postId, default))
                .ReturnsAsync(post);

            await _service.AddLikeAsync(userId, postId);

            _likeRepoMock.Verify(x => x.AddAsync(
                It.Is<Like>(l => l.UserId == userId && l.PostId == postId),
                default
            ), Times.Once);
            _postRepoMock.Verify(x => x.UpdateAsync(
                It.Is<Post>(p => p.Id == postId && p.LikeCount == 1),
                default
            ), Times.Once);
        }

        [Fact]
        public async Task RemoveLike_ShouldRemoveLikeAndDecrementCount()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId, LikeCount = 1 };

            _postRepoMock.Setup(x => x.GetByIdAsync(postId, default))
                .ReturnsAsync(post);

            await _service.RemoveLikeAsync(userId, postId);

            _likeRepoMock.Verify(x => x.DeleteAsync(userId, postId, default), Times.Once);
            _postRepoMock.Verify(x => x.UpdateAsync(
                It.Is<Post>(p => p.Id == postId && p.LikeCount == 0),
                default
            ), Times.Once);
        }

        [Fact]
        public async Task GetLikes_ShouldReturnMappedUsers()
        {
            var postId = Guid.NewGuid();
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid() },
                new User { Id = Guid.NewGuid() }
            };
            var expectedDtos = users.Select(u => new UserProfileDTO { Id = u.Id }).ToList();

            _likeRepoMock.Setup(x => x.GetLikesAsync(postId, default))
                .ReturnsAsync(users);
            _mapperMock.Setup(x => x.Map<IEnumerable<UserProfileDTO>>(users))
                .Returns(expectedDtos);

            var result = await _service.GetLikesAsync(postId);

            Assert.Equal(expectedDtos.Count, result.Count());
            Assert.Equal(expectedDtos.First().Id, result.First().Id);
        }

        [Fact]
        public async Task AddRepost_WhenRepostDoesNotExist_ShouldAddRepostAndIncrementCount()
        {
            var userId = Guid.NewGuid();
            var dto = new RepostCreateDTO
            {
                PostId = Guid.NewGuid(),
                Comment = "Test repost"
            };
            var post = new Post { Id = dto.PostId, RepostCount = 0 };

            _repostRepoMock.Setup(x => x.GetByIdAsync(userId, dto.PostId, default))
                .ReturnsAsync((Repost)null);
            _postRepoMock.Setup(x => x.GetByIdAsync(dto.PostId, default))
                .ReturnsAsync(post);

            await _service.AddRepostAsync(userId, dto);

            _repostRepoMock.Verify(x => x.AddAsync(
                It.Is<Repost>(r =>
                    r.UserId == userId &&
                    r.PostId == dto.PostId &&
                    r.Comment == dto.Comment),
                default
            ), Times.Once);
            _postRepoMock.Verify(x => x.UpdateAsync(
                It.Is<Post>(p => p.Id == dto.PostId && p.RepostCount == 1),
                default
            ), Times.Once);
        }

        [Fact]
        public async Task GetReposts_ShouldReturnMappedPosts()
        {
            var postId = Guid.NewGuid();
            var posts = new List<Post>
            {
                new Post { Id = Guid.NewGuid() },
                new Post { Id = Guid.NewGuid() }
            };
            var expectedDtos = posts.Select(p => new PostDTO { Id = p.Id }).ToList();

            _repostRepoMock.Setup(x => x.GetRepostsAsync(postId, default))
                .ReturnsAsync(posts);
            _mapperMock.Setup(x => x.Map<IEnumerable<PostDTO>>(posts))
                .Returns(expectedDtos);

            var result = await _service.GetRepostsAsync(postId);

            Assert.Equal(expectedDtos.Count, result.Count());
            Assert.Equal(expectedDtos.First().Id, result.First().Id);
        }

        [Fact]
        public async Task UnsubscribeAsync_ShouldCallDeleteAsync()
        {
            var followerId = Guid.NewGuid();
            var followingId = Guid.NewGuid();

            await _service.UnsubscribeAsync(followerId, followingId);

            _subscriptionRepoMock.Verify(x => x.DeleteAsync(followerId, followingId, default), Times.Once);
        }

        [Fact]
        public async Task GetFollowers_ShouldReturnMappedUsers()
        {
            var userId = Guid.NewGuid();
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid() },
                new User { Id = Guid.NewGuid() }
            };
            var expectedDtos = users.Select(u => new UserProfileDTO { Id = u.Id }).ToList();

            _subscriptionRepoMock.Setup(x => x.GetFollowersAsync(userId, default))
                .ReturnsAsync(users);
            _mapperMock.Setup(x => x.Map<IEnumerable<UserProfileDTO>>(users))
                .Returns(expectedDtos);

            var result = await _service.GetFollowersAsync(userId);

            Assert.Equal(expectedDtos.Count, result.Count());
            Assert.Equal(expectedDtos.First().Id, result.First().Id);
        }

        [Fact]
        public async Task GetFollowing_ShouldReturnMappedUsers()
        {
            var userId = Guid.NewGuid();
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid() },
                new User { Id = Guid.NewGuid() }
            };
            var expectedDtos = users.Select(u => new UserProfileDTO { Id = u.Id }).ToList();

            _subscriptionRepoMock.Setup(x => x.GetFollowingAsync(userId, default))
                .ReturnsAsync(users);
            _mapperMock.Setup(x => x.Map<IEnumerable<UserProfileDTO>>(users))
                .Returns(expectedDtos);

            var result = await _service.GetFollowingAsync(userId);

            Assert.Equal(expectedDtos.Count, result.Count());
            Assert.Equal(expectedDtos.First().Id, result.First().Id);
        }

        [Fact]
        public async Task GetCommentsByPost_ShouldReturnCommentTree()
        {
            var postId = Guid.NewGuid();
            var parentId = Guid.NewGuid();
            var replyId = Guid.NewGuid();

            var comments = new List<Comment>
            {
                new Comment
                {
                    Id = parentId,
                    ParentCommentId = null,
                    CreatedAt = DateTime.UtcNow,
                    PostId = postId,
                    Content = "Parent comment"
                },
                new Comment
                {
                    Id = replyId,
                    ParentCommentId = parentId,
                    CreatedAt = DateTime.UtcNow.AddMinutes(1),
                    PostId = postId,
                    Content = "Reply comment"
                }
            };

            var parentDto = new CommentDTO
            {
                Id = parentId,
                PostId = postId,
                Content = "Parent comment",
                CreatedAt = comments[0].CreatedAt,
                Replies = new List<CommentDTO>()
            };

            var replyDto = new CommentDTO
            {
                Id = replyId,
                PostId = postId,
                Content = "Reply comment",
                CreatedAt = comments[1].CreatedAt,
                Replies = new List<CommentDTO>(),
                Parent = parentDto
            };

            _commentRepoMock.Setup(x => x.GetCommentsByPostAsync(postId, default))
                .ReturnsAsync(comments);

            _mapperMock.Setup(x => x.Map<CommentDTO>(It.Is<Comment>(c => c.Id == parentId)))
                .Returns(parentDto);
            _mapperMock.Setup(x => x.Map<CommentDTO>(It.Is<Comment>(c => c.Id == replyId)))
                .Returns(replyDto);

            var result = await _service.GetCommentsByPostAsync(postId);

            Assert.Single(result);
            var parentComment = result.First();
            Assert.Single(parentComment.Replies);
            var reply = parentComment.Replies.First();
            Assert.Equal(parentComment, reply.Parent);
        }
    }
}