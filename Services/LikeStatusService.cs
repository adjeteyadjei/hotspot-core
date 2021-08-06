using AutoMapper;
using hotvenues.Models;
using Hotvenues.Data;
using Hotvenues.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Hotvenues.Services
{
    public class LikeStatusDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
    }

    public interface ILikeStatusService : IModelService<LikeStatusDto>
    {
        Task<bool> LikeStatus(long id, string username);
    }

    public class LikeStatusService : BaseService<LikeStatusDto, LikeStatus>, ILikeStatusService
    {
        public LikeStatusService(AppDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<bool> LikeStatus(long id, string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(q => q.UserName == username);
            if (user == null) throw new Exception("Unknown Account");

            var status = await _context.StatusLikes.AnyAsync(q => q.StatusId == id && q.UserId == user.Id);
            if (status) throw new Exception("Already Liked");

            var statusLike = new LikeStatus
            {
                Timestamp = DateTime.UtcNow,
                UserId = user.Id,
                StatusId = id
            };

            _context.StatusLikes.Add(statusLike);
            _context.SaveChanges();

            return true;
        }

    }
}
