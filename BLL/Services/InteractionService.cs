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

        public async Task AddBookmarkAsync(Guid userId, Guid postId)
        {
            var exists = await _bookmarkRepo.GetBookmarkAsync(userId, postId);
            if (exists != null) return;

            await _bookmarkRepo.AddBookmarkAsync(userId, postId);
            await _postRepo.IncrementBookmarkCountAsync(postId);
        }

        public async Task RemoveBookmarkAsync(Guid userId, Guid postId)
        {
            await _bookmarkRepo.RemoveBookmarkAsync(userId, postId);
            await _postRepo.DecrementBookmarkCountAsync(postId);
        }

        public async Task<IEnumerable<PostDTO>> GetBookmarksAsync(Guid userId)
        {
            var bookmarks = await _bookmarkRepo.GetBookmarksAsync(userId);
            return _mapper.Map<IEnumerable<PostDTO>>(bookmarks);
        }

        public async Task<CommentDTO> AddCommentAsync(CommentCreateDTO dto, Guid authorId)
        {
            var comment = new Comment
            {
                PostId = dto.PostId,
                AuthorId = authorId,
                ParentCommentId = dto.ParentCommentId,
                Content = dto.Content
            };

            var created = await _commentRepo.AddCommentAsync(comment);
            await _postRepo.IncrementCommentCountAsync(dto.PostId);
            return _mapper.Map<CommentDTO>(created);
        }

        public async Task DeleteCommentAsync(Guid commentId, Guid authorId)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId);
            if (comment == null) return;
            if (comment.AuthorId != authorId)
                throw new UnauthorizedAccessException();

            await _commentRepo.DeleteCommentAsync(commentId);
            await _postRepo.DecrementCommentCountAsync(comment.PostId);
        }

        public async Task<IEnumerable<CommentDTO>> GetCommentsByPostAsync(Guid postId)
        {
            var comments = await _commentRepo.GetCommentsByPostAsync(postId);
            return BuildCommentTree(comments);
        }

        public async Task AddLikeAsync(Guid userId, Guid postId)
        {
            var exists = await _likeRepo.GetLikeAsync(userId, postId);
            if (exists != null) return;

            await _likeRepo.AddLikeAsync(userId, postId);
            await _postRepo.IncrementLikeCountAsync(postId);
        }

        public async Task RemoveLikeAsync(Guid userId, Guid postId)
        {
            await _likeRepo.RemoveLikeAsync(userId, postId);
            await _postRepo.DecrementLikeCountAsync(postId);
        }

        public async Task<IEnumerable<UserProfileDTO>> GetLikesAsync(Guid postId)
        {
            var users = await _likeRepo.GetLikesAsync(postId);
            return _mapper.Map<IEnumerable<UserProfileDTO>>(users);
        }

        public async Task AddRepostAsync(Guid userId, RepostCreateDTO dto)
        {
            var exists = await _repostRepo.GetRepostAsync(userId, dto.PostId);
            if (exists != null) return;

            await _repostRepo.AddRepostAsync(userId, dto.PostId, dto.Comment);
            await _postRepo.IncrementRepostCountAsync(dto.PostId);
        }

        public async Task<IEnumerable<PostDTO>> GetRepostsAsync(Guid postId)
        {
            var reposts = await _repostRepo.GetRepostsAsync(postId);
            return _mapper.Map<IEnumerable<PostDTO>>(reposts);
        }

        public async Task SubscribeAsync(Guid followerId, Guid followingId)
        {
            if (followerId == followingId)
                throw new ArgumentException("Cannot subscribe to yourself");

            var exists = await _subscriptionRepo.GetSubscriptionAsync(followerId, followingId);
            if (exists != null) return;

            await _subscriptionRepo.SubscribeAsync(followerId, followingId);
        }

        public async Task UnsubscribeAsync(Guid followerId, Guid followingId)
        {
            await _subscriptionRepo.UnsubscribeAsync(followerId, followingId);
        }

        public async Task<IEnumerable<UserProfileDTO>> GetFollowersAsync(Guid userId)
        {
            var users = await _subscriptionRepo.GetFollowersAsync(userId);
            return _mapper.Map<IEnumerable<UserProfileDTO>>(users);
        }

        public async Task<IEnumerable<UserProfileDTO>> GetFollowingAsync(Guid userId)
        {
            var users = await _subscriptionRepo.GetFollowingAsync(userId);
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