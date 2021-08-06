using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hotvenues.Data;
using Hotvenues.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hotvenues.Models;
using System;
using Hotvenues.Helpers;

namespace Hotvenues.Services
{
    public class LiveStatusDto
    {
        public string Username { get; set; }
        public string Media { get; set; }
        public string Caption { get; set; }
        public string RootPath { get; set; }
    }

    public class LiveStatusCardDto
    {
        public long Id { get; set; }
        public string Vendor { get; set; }
        public DateTime Timestamp { get; set; }
        public string Media { get; set; }
        public string Caption { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
        public string MediaType { get; set; }
        public string MediaExtension { get; set; }
        public ICollection<LikeStatus> LikeStatus { get; set; }
        public string Picture { get; set; }
    }

    public class LiveStatusFilter
    {
        [FromQuery]
        public string Text { get; set; }

        public string RootPath { get; set; }

        [FromQuery(Name = "_page")]
        public int Page { get; set; }

        [FromQuery(Name = "_size")]
        public int Size { get; set; }

        public int Skip() { return (Page - 1) * Size; }

        public IQueryable<LiveStatus> BuildQuery(IQueryable<LiveStatus> query)
        {
            //if (!string.IsNullOrEmpty(Name)) query = query.Where(q => q.Name.Contains(Name));
            return query;
        }
    }

    public interface ILiveStatusService : IModelService<LiveStatusDto>
    {
        Task<List<LiveStatusCardDto>> Query(LiveStatusFilter filter);
        Task<string> GetMediaExtension(string fileName);
        Task<List<LiveStatusCardDto>> TopPosts();
    }

    public class LiveStatusService : BaseService<LiveStatusDto, LiveStatus>, ILiveStatusService
    {
        public LiveStatusService(AppDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<long> Save(LiveStatusDto record)
        {
            var vendor = await _context.Users.FirstOrDefaultAsync(q => q.UserName == record.Username);
            if (vendor == null) throw new Exception("Unknown Account");

            var (filename, extension) = new MediaHelper(record.RootPath).SaveDataMedia(record.Media);
            var status = new LiveStatus
            {
                Timestamp = DateTime.UtcNow,
                Media = filename,
                MediaExtension = extension,
                Caption = record.Caption,
                VendorId = vendor.Id
            };

            _context.LiveStatuses.Add(status);
            _context.SaveChanges();

            return status.Id;
        }

        public async Task<List<LiveStatusCardDto>> Query(LiveStatusFilter filter)
        {
            var data = await filter.BuildQuery(_context.LiveStatuses.Select(x => x))
                .Include(x => x.Vendor)
                .Include(x => x.Likes)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.Id)
                .Skip(filter.Skip()).Take(filter.Size)
                .ToListAsync();

            return data.Select(x => new LiveStatusCardDto
            {
                Id = x.Id,
                Vendor = x.Vendor.Name,
                Media = x.Media,
                Caption = x.Caption,
                Timestamp = x.Timestamp,
                Likes = x.Likes.Count(),
                LikeStatus = x.Likes.Select(x => new LikeStatus { Id = x.Id, UserId = x.UserId, StatusId = x.StatusId }).ToList(),
                Comments = x.Comments.Count(),
                MediaExtension = x.MediaExtension,
                Picture = x.Vendor.Picture
            }).ToList();
        }

        public async Task<string> GetMediaExtension(string fileName)
        {
            var ext = await _context.LiveStatuses.FirstOrDefaultAsync(q => q.Media == fileName);

            return ext.MediaExtension;
        }

        public async Task<List<LiveStatusCardDto>> TopPosts()
        {
            var today = DateTime.UtcNow.AddHours(-5);
            return await _context.LiveStatuses.Where(q => q.Timestamp >= today)
                .OrderByDescending(q => q.Likes.Count)
                .Take(10).Select(x => new LiveStatusCardDto
                {
                    Id = x.Id,
                    Vendor = x.Vendor.Name,
                    Media = x.Media,
                    Caption = x.Caption,
                    Timestamp = x.Timestamp,
                    Likes = x.Likes.Count(),
                    Comments = x.Comments.Count(),
                    MediaExtension = x.MediaExtension,
                    Picture = x.Vendor.Picture
                })
                .ToListAsync();
        }

    }
}
