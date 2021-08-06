using AutoMapper;
using hotvenues.Models;
using Hotvenues.Data;
using Hotvenues.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotvenues.Services
{
    public class CommentStatusDto
    {
        public long Id { get; set; }
        public long StatusId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
    }

    public interface ICommentStatusService : IModelService<CommentStatusDto>
    {
        Task<long> CommentStatus(long id, string username, string comment);
        Task<List<CommentStatus>> Comments(long id);
    }

    public class CommentStatusService : BaseService<CommentStatusDto, CommentStatus>, ICommentStatusService
    {
        public CommentStatusService(AppDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<long> CommentStatus(long id, string username, string comment)
        {
            var user = await _context.Users.FirstOrDefaultAsync(q => q.UserName == username);
            
            if (user == null) throw new Exception("Unknown Account");

            var statusComment = new CommentStatus
            {
                Timestamp = DateTime.UtcNow,
                UserId = user.Id,
                StatusId = id,
                Text = comment
            };

            _context.StatusComments.Add(statusComment);
            _context.SaveChanges();

            return statusComment.Id;
        }

        public async Task<List<CommentStatus>> Comments(long id)
        {
            return await _context.StatusComments.Where(x => x.StatusId == id)
                .Include(x => x.User)
                .Select(q => new CommentStatus
                {
                    Id = q.Id,
                    Text = q.Text,
                    Timestamp = q.Timestamp,
                    User = q.User,
                    UserId = q.UserId,
                    StatusId = q.StatusId,
                }).ToListAsync();
        }
    }
}
