using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services
{
    public class FeedService : IFeedService
    {
        private readonly IFeedRepository _feedRepo;
        private readonly ILikeRepository _likeRepo;
        private readonly IBookmarkRepository _bookmarkRepo;
        private readonly IRepostRepository _repostRepo;
        private readonly IMapper _mapper;

        public FeedService(
            IFeedRepository feedRepo,
            ILikeRepository likeRepo,
            IBookmarkRepository bookmarkRepo,
            IRepostRepository repostRepo,
            IMapper mapper)
        {
            _feedRepo = feedRepo;
            _likeRepo = likeRepo;
            _bookmarkRepo = bookmarkRepo;
            _repostRepo = repostRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PostDTO>> GetFeedAsync(
            Guid userId,
            int page = 1,
            int pageSize = 10,
            string filter = "recent")
        {
            var posts = await _feedRepo.GetFeedAsync(userId, page, pageSize, filter);
            var postDtos = _mapper.Map<IEnumerable<PostDTO>>(posts);

            foreach (var post in postDtos)
            {
                post.IsLiked = await _likeRepo.ExistsAsync(userId, post.Id);
                post.IsBookmarked = await _bookmarkRepo.ExistsAsync(userId, post.Id);
                post.IsReposted = await _repostRepo.ExistsAsync(userId, post.Id);
            }

            return postDtos;
        }
    }
}