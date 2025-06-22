using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using System.Threading;

namespace BLL.Services
{
    public class FeedService : IFeedService
    {
        private readonly IFeedRepository _feedRepo;
        private readonly ILikeRepository _likeRepo;
        private readonly IBookmarkRepository _bookmarkRepo;
        private readonly IRepostRepository _repostRepo;
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly IMapper _mapper;

        public FeedService(
            IFeedRepository feedRepo,
            ILikeRepository likeRepo,
            IBookmarkRepository bookmarkRepo,
            IRepostRepository repostRepo,
            ISubscriptionRepository subscriptionRepo,
            IMapper mapper)
        {
            _feedRepo = feedRepo;
            _likeRepo = likeRepo;
            _bookmarkRepo = bookmarkRepo;
            _repostRepo = repostRepo;
            _subscriptionRepo = subscriptionRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PostDTO>> GetFeedAsync(Guid userId, int page = 1, int pageSize = 10, string filter = "recent", CancellationToken cancellationToken = default)
        {
            IEnumerable<Post> posts;

            if (filter == "subscriptions")
            {
                var followingUsers = await _subscriptionRepo.GetFollowingAsync(userId, cancellationToken);
                var followingIds = followingUsers.Select(u => u.Id);
                posts = await _feedRepo.GetPostsByAuthorsAsync(followingIds, page, pageSize);
            }
            else if (filter == "popular")
            {
                var since = DateTime.UtcNow.AddDays(-7);
                posts = await _feedRepo.GetPopularPostsAsync(since, page, pageSize);
            }
            else
            {
                posts = await _feedRepo.GetRecentPostsAsync(page, pageSize);
            }

            var postDtos = _mapper.Map<List<PostDTO>>(posts);

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
