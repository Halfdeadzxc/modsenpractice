using AutoMapper;
using BLL.DTO;
using BLL.Services;
using DAL.Interfaces;
using DAL.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.BLLTests
{
    public class PostServiceTests
    {
        private readonly Mock<IPostRepository> _repo = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly PostService _service;
        private readonly CancellationToken _ct = CancellationToken.None;

        public PostServiceTests()
        {
            _service = new PostService(_repo.Object, _mapper.Object);
        }

        [Fact]
        public async Task CreatePostAsync_SetsAuthorAndHashtagsAndReturnsDto()
        {
            var authorId = Guid.NewGuid();
            var dto = new PostCreateDTO
            {
                Content = "Hello #X #y #x",
                MediaUrls = new List<string> { "u1", "u2" }
            };
            var mappedPost = new Post { Content = dto.Content, MediaUrls = dto.MediaUrls.ToList() };
            var createdPost = new Post
            {
                Id = Guid.NewGuid(),
                AuthorId = authorId,
                Content = dto.Content,
                MediaUrls = dto.MediaUrls.ToList(),
                Hashtags = "#x #y"
            };
            var returnedDto = new PostDTO { Id = createdPost.Id, Content = createdPost.Content, Hashtags = createdPost.Hashtags };

            _mapper
                .Setup(m => m.Map<Post>(dto))
                .Returns(mappedPost);
            _repo
                .Setup(r => r.AddAsync(mappedPost, _ct))
                .ReturnsAsync(createdPost);
            _mapper
                .Setup(m => m.Map<PostDTO>(createdPost))
                .Returns(returnedDto);

            var result = await _service.CreatePostAsync(authorId, dto, _ct);

            Assert.Equal(returnedDto, result);
            Assert.Equal(authorId, createdPost.AuthorId);
            Assert.Equal("#x #y", createdPost.Hashtags);
        }

        [Fact]
        public async Task UpdatePostAsync_NotFound_ThrowsKeyNotFound()
        {
            var postId = Guid.NewGuid();
            _repo
                .Setup(r => r.GetByIdAsync(postId, _ct))
                .ReturnsAsync((Post)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.UpdatePostAsync(postId, Guid.NewGuid(), new PostCreateDTO(), _ct)
            );
        }

        [Fact]
        public async Task UpdatePostAsync_WrongUser_ThrowsUnauthorized()
        {
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId, AuthorId = Guid.NewGuid() };
            _repo
                .Setup(r => r.GetByIdAsync(postId, _ct))
                .ReturnsAsync(post);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.UpdatePostAsync(postId, Guid.NewGuid(), new PostCreateDTO(), _ct)
            );
        }

        [Fact]
        public async Task UpdatePostAsync_Valid_UpdatesFieldsAndReturnsDto()
        {
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new PostCreateDTO
            {
                Content = "New #Tag",
                MediaUrls = new List<string> { "url" }
            };
            var original = new Post
            {
                Id = postId,
                AuthorId = userId,
                Content = "Old",
                MediaUrls = new List<string>(),
                Hashtags = ""
            };
            var updatedPost = new Post
            {
                Id = postId,
                AuthorId = userId,
                Content = dto.Content,
                MediaUrls = dto.MediaUrls.ToList(),
                Hashtags = "#tag"
            };
            var expectedDto = new PostDTO { Id = postId, Content = dto.Content, Hashtags = "#tag" };

            _repo.Setup(r => r.GetByIdAsync(postId, _ct)).ReturnsAsync(original);
            _repo.Setup(r => r.UpdateAsync(It.Is<Post>(p =>
                    p.Id == postId &&
                    p.Content == dto.Content &&
                    p.MediaUrls.SequenceEqual(dto.MediaUrls) &&
                    p.Hashtags == "#tag"), _ct))
                 .ReturnsAsync(updatedPost);
            _mapper.Setup(m => m.Map<PostDTO>(updatedPost)).Returns(expectedDto);

            var result = await _service.UpdatePostAsync(postId, userId, dto, _ct);

            Assert.Equal(expectedDto, result);
        }

        [Fact]
        public async Task DeletePostAsync_NotFound_DoesNothing()
        {
            var postId = Guid.NewGuid();
            _repo.Setup(r => r.GetByIdAsync(postId, _ct)).ReturnsAsync((Post)null);

            await _service.DeletePostAsync(postId, Guid.NewGuid(), _ct);

            _repo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeletePostAsync_WrongUser_ThrowsUnauthorized()
        {
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId, AuthorId = Guid.NewGuid() };
            _repo.Setup(r => r.GetByIdAsync(postId, _ct)).ReturnsAsync(post);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.DeletePostAsync(postId, Guid.NewGuid(), _ct)
            );

            _repo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeletePostAsync_Valid_CallsDelete()
        {
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var post = new Post { Id = postId, AuthorId = userId };
            _repo.Setup(r => r.GetByIdAsync(postId, _ct)).ReturnsAsync(post);
            _repo.Setup(r => r.DeleteAsync(postId, _ct)).Returns(Task.CompletedTask);

            await _service.DeletePostAsync(postId, userId, _ct);

            _repo.Verify(r => r.DeleteAsync(postId, _ct), Times.Once);
        }

        [Fact]
        public async Task GetPostByIdAsync_ReturnsMappedDto()
        {
            var post = new Post { Id = Guid.NewGuid() };
            var dto = new PostDTO { Id = post.Id };
            _repo.Setup(r => r.GetByIdAsync(post.Id, _ct)).ReturnsAsync(post);
            _mapper.Setup(m => m.Map<PostDTO>(post)).Returns(dto);

            var result = await _service.GetPostByIdAsync(post.Id, _ct);

            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task GetPostsByUserAsync_ReturnsMappedDtos()
        {
            var userId = Guid.NewGuid();
            var posts = new List<Post> { new() { Id = Guid.NewGuid() } };
            var dtos = new List<PostDTO> { new() { Id = posts[0].Id } };
            _repo.Setup(r => r.GetByUserAsync(userId, _ct)).ReturnsAsync(posts);
            _mapper.Setup(m => m.Map<IEnumerable<PostDTO>>(posts)).Returns(dtos);

            var result = await _service.GetPostsByUserAsync(userId, _ct);

            Assert.Equal(dtos, result);
        }

        [Fact]
        public async Task GetPostsByHashtagAsync_ReturnsMappedDtos()
        {
            var tag = "#x";
            var posts = new List<Post> { new() { Id = Guid.NewGuid() } };
            var dtos = new List<PostDTO> { new() { Id = posts[0].Id } };
            _repo.Setup(r => r.GetByHashtagAsync(tag, _ct)).ReturnsAsync(posts);
            _mapper.Setup(m => m.Map<IEnumerable<PostDTO>>(posts)).Returns(dtos);

            var result = await _service.GetPostsByHashtagAsync(tag, _ct);

            Assert.Equal(dtos, result);
        }
    }
}
