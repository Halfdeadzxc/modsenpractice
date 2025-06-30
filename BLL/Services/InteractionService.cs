using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services
{
    public class InteractionService : IInteractionService
    {
        private readonly IBookmarkRepository _bookmarkRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly ILikeRepository _likeRepo;
        private readonly IRepostRepository _repostRepo;
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly IPostRepository _postRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public InteractionService(
            IBookmarkRepository bookmarkRepo,
            ICommentRepository commentRepo,
            ILikeRepository likeRepo,
            IRepostRepository repostRepo,
            ISubscriptionRepository subscriptionRepo,
            IPostRepository postRepo,
            IUserRepository userRepo,
            IMapper mapper)
        {
            _bookmarkRepo = bookmarkRepo;
            _commentRepo = commentRepo;
            _likeRepo = likeRepo;
            _repostRepo = repostRepo;
            _subscriptionRepo = subscriptionRepo;
            _postRepo = postRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task AddBookmarkAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
        {
            var exists = await _bookmarkRepo.GetByIdAsync(userId, postId, cancellationToken);
            if (exists is not null)
            {
                return;
            }

            var bookmark = new Bookmark { UserId = userId, PostId = postId };
            await _bookmarkRepo.AddAsync(bookmark, cancellationToken);

            var post = await _postRepo.GetByIdAsync(postId, cancellationToken);
            if (post is not null)
            {
                post.BookmarkCount++;
                await _postRepo.UpdateAsync(post, cancellationToken);
            }
        }

        public async Task RemoveBookmarkAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
        {
            await _bookmarkRepo.DeleteAsync(userId, postId, cancellationToken);

            var post = await _postRepo.GetByIdAsync(postId, cancellationToken);
            if (post is not null && post.BookmarkCount > 0)
            {
                post.BookmarkCount--;
                await _postRepo.UpdateAsync(post, cancellationToken);
            }
        }

        public async Task<IEnumerable<PostDTO>> GetBookmarksAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var bookmarks = await _bookmarkRepo.GetBookmarksAsync(userId, cancellationToken);
            return _mapper.Map<IEnumerable<PostDTO>>(bookmarks);
        }

        public async Task<CommentDTO> AddCommentAsync(CommentCreateDTO dto, Guid authorId, CancellationToken cancellationToken = default)
        {
            var comment = new Comment
            {
                PostId = dto.PostId,
                AuthorId = authorId,
                ParentCommentId = dto.ParentCommentId,
                Content = dto.Content
            };

            var created = await _commentRepo.AddAsync(comment, cancellationToken);

            var post = await _postRepo.GetByIdAsync(dto.PostId, cancellationToken);
            if (post is not null)
            {
                post.CommentCount++;
                await _postRepo.UpdateAsync(post, cancellationToken);
            }

            return _mapper.Map<CommentDTO>(created);
        }

        public async Task DeleteCommentAsync(Guid commentId, Guid authorId, CancellationToken cancellationToken = default)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId, cancellationToken);
            if (comment is null)
            {
                return;
            }

            if (comment.AuthorId != authorId)
            {
                throw new UnauthorizedAccessException();
            }

            await _commentRepo.DeleteAsync(commentId, cancellationToken);

            var post = await _postRepo.GetByIdAsync(comment.PostId, cancellationToken);
            if (post is not null && post.CommentCount > 0)
            {
                post.CommentCount--;
                await _postRepo.UpdateAsync(post, cancellationToken);
            }
        }

        public async Task<IEnumerable<CommentDTO>> GetCommentsByPostAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            var comments = await _commentRepo.GetCommentsByPostAsync(postId, cancellationToken);
            return BuildCommentTree(comments);
        }

        public async Task AddLikeAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
        {
            var exists = await _likeRepo.GetByIdAsync(userId, postId, cancellationToken);
            if (exists is not null)
            {
                return;
            }

            var like = new Like { UserId = userId, PostId = postId };
            await _likeRepo.AddAsync(like, cancellationToken);

            var post = await _postRepo.GetByIdAsync(postId, cancellationToken);
            if (post is not null)
            {
                post.LikeCount++;
                await _postRepo.UpdateAsync(post, cancellationToken);
            }
        }

        public async Task RemoveLikeAsync(Guid userId, Guid postId, CancellationToken cancellationToken = default)
        {
            await _likeRepo.DeleteAsync(userId, postId, cancellationToken);

            var post = await _postRepo.GetByIdAsync(postId, cancellationToken);
            if (post is not null && post.LikeCount > 0)
            {
                post.LikeCount--;
                await _postRepo.UpdateAsync(post, cancellationToken);
            }
        }

        public async Task<IEnumerable<UserProfileDTO>> GetLikesAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            var users = await _likeRepo.GetLikesAsync(postId, cancellationToken);
            return _mapper.Map<IEnumerable<UserProfileDTO>>(users);
        }

        public async Task AddRepostAsync(Guid userId, RepostCreateDTO dto, CancellationToken cancellationToken = default)
        {
            var exists = await _repostRepo.GetByIdAsync(userId, dto.PostId, cancellationToken);
            if (exists is not null)
            {
                return;
            }

            var repost = new Repost
            {
                UserId = userId,
                PostId = dto.PostId,
                Comment = dto.Comment
            };

            await _repostRepo.AddAsync(repost, cancellationToken);

            var post = await _postRepo.GetByIdAsync(dto.PostId, cancellationToken);
            if (post is not null)
            {
                post.RepostCount++;
                await _postRepo.UpdateAsync(post, cancellationToken);
            }
        }

        public async Task<IEnumerable<PostDTO>> GetRepostsAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            var reposts = await _repostRepo.GetRepostsAsync(postId, cancellationToken);
            return _mapper.Map<IEnumerable<PostDTO>>(reposts);
        }

        public async Task SubscribeAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default)
        {
            if (followerId == followingId)
            {
                throw new ArgumentException("Cannot subscribe to yourself");
            }

            var exists = await _subscriptionRepo.GetByIdAsync(followerId, followingId, cancellationToken);
            if (exists is not null)
            {
                return;
            }

            var subscription = new Subscription
            {
                FollowerId = followerId,
                FollowingId = followingId,
                Status = "approved"
            };

            await _subscriptionRepo.AddAsync(subscription, cancellationToken);
        }

        public async Task UnsubscribeAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default)
        {
            await _subscriptionRepo.DeleteAsync(followerId, followingId, cancellationToken);
        }

        public async Task<IEnumerable<UserProfileDTO>> GetFollowersAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var users = await _subscriptionRepo.GetFollowersAsync(userId, cancellationToken);
            return _mapper.Map<IEnumerable<UserProfileDTO>>(users);
        }

        public async Task<IEnumerable<UserProfileDTO>> GetFollowingAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var users = await _subscriptionRepo.GetFollowingAsync(userId, cancellationToken);
            return _mapper.Map<IEnumerable<UserProfileDTO>>(users);
        }

        private IEnumerable<CommentDTO> BuildCommentTree(IEnumerable<Comment> comments)
        {
            var commentMap = comments.ToDictionary(c => c.Id);
            var rootComments = new List<CommentDTO>();

            foreach (var comment in comments.Where(c => c.ParentCommentId == null))
            {
                var dto = _mapper.Map<CommentDTO>(comment);
                BuildReplies(dto, commentMap);
                rootComments.Add(dto);
            }

            return rootComments.OrderByDescending(c => c.CreatedAt);
        }

        private void BuildReplies(CommentDTO parent, Dictionary<Guid, Comment> commentMap)
        {
            var replies = commentMap.Values
                .Where(c => c.ParentCommentId == parent.Id)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            foreach (var reply in replies)
            {
                var replyDto = _mapper.Map<CommentDTO>(reply);

                if (GetNestedLevel(parent) < 3)
                {
                    BuildReplies(replyDto, commentMap);
                }

                parent.Replies.Add(replyDto);
            }
        }

        private int GetNestedLevel(CommentDTO comment)
        {
            int level = 0;

            while (comment.Parent != null)
            {
                level++;
                comment = comment.Parent;
            }

            return level;
        }

    }
}