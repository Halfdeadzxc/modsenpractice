using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepo;
        private readonly IMapper _mapper;

        public PostService(IPostRepository postRepo, IMapper mapper)
        {
            _postRepo = postRepo;
            _mapper = mapper;
        }

        public async Task<PostDTO> CreatePostAsync(Guid authorId, PostCreateDTO dto, CancellationToken cancellationToken = default)
        {
            var post = _mapper.Map<Post>(dto);
            post.AuthorId = authorId;
            post.Hashtags = ExtractHashtags(dto.Content);

            var created = await _postRepo.AddAsync(post, cancellationToken);
            return _mapper.Map<PostDTO>(created);
        }

        public async Task<PostDTO> UpdatePostAsync(Guid postId, Guid userId, PostCreateDTO dto, CancellationToken cancellationToken = default)
        {
            var post = await _postRepo.GetByIdAsync(postId, cancellationToken);
            if (post is null)
            {
                throw new KeyNotFoundException("Post not found"); 
            }
            if (post.AuthorId != userId) throw new UnauthorizedAccessException();

            post.Content = dto.Content;
            post.MediaUrls = dto.MediaUrls;
            post.Hashtags = ExtractHashtags(dto.Content);
            post.UpdatedAt = DateTime.UtcNow;

            var updated = await _postRepo.UpdateAsync(post, cancellationToken);
            return _mapper.Map<PostDTO>(updated);
        }

        public async Task DeletePostAsync(Guid postId, Guid userId, CancellationToken cancellationToken = default)
        {
            var post = await _postRepo.GetByIdAsync(postId, cancellationToken);
            if (post is null) return;
            if (post.AuthorId != userId) throw new UnauthorizedAccessException();

            await _postRepo.DeleteAsync(postId, cancellationToken);
        }

        public async Task<PostDTO> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            var post = await _postRepo.GetByIdAsync(postId, cancellationToken);
            return _mapper.Map<PostDTO>(post);
        }

        public async Task<IEnumerable<PostDTO>> GetPostsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var posts = await _postRepo.GetByUserAsync(userId, cancellationToken);
            return _mapper.Map<IEnumerable<PostDTO>>(posts);
        }

        public async Task<IEnumerable<PostDTO>> GetPostsByHashtagAsync(string hashtag, CancellationToken cancellationToken = default)
        {
            var posts = await _postRepo.GetByHashtagAsync(hashtag, cancellationToken);
            return _mapper.Map<IEnumerable<PostDTO>>(posts);
        }

        private static string ExtractHashtags(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            var matches = Regex.Matches(content, @"#\w+")
                .Select(m => m.Value.ToLower())
                .Distinct();

            return string.Join(" ", matches);
        }
    }
}
