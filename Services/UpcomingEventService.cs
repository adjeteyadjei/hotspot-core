using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hotvenues.Data;
using Hotvenues.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using hotvenues.Models;
using Hotvenues.Helpers;
using System.Globalization;

namespace Hotvenues.Services
{
    public class UpcomingEventDto
    {
        public string Username { get; set; }
        public string Media { get; set; }
        public string Theme { get; set; }
        public string Location { get; set; }
        public DateTime DateTime { get; set; }
        public string RootPath { get; set; }
    }

    public class UpcomingEventCardDto 
    {
        public long Id { get; set; }
        public string Vendor { get; set; }
        public DateTime DateTime { get; set; }
        public string Month { get; set; }
        public string Number { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public string Media { get; set; }
        public string Theme { get; set; }
        public string Location { get; set; }
        public string MediaType { get; set; }
        public string MediaExtension { get; set; }
    }

    public class UpcomingEventFilter
    {
        [FromQuery]
        public string Text { get; set; }

        public string RootPath { get; set; }

        [FromQuery(Name = "_page")]
        public int Page { get; set; }

        [FromQuery(Name = "_size")]
        public int Size { get; set; }

        public int Skip() { return (Page - 1) * Size; }

        public IQueryable<UpcomingEvent> BuildQuery(IQueryable<UpcomingEvent> query)
        {
            //if (!string.IsNullOrEmpty(Name)) query = query.Where(q => q.Name.Contains(Name));
            return query;
        }
    }

    public interface IUpcomingEventService : IModelService<UpcomingEventDto>
    {
        Task<List<UpcomingEventCardDto>> Query(UpcomingEventFilter filter);
        Task<string> GetMediaExtension(string fileName);
    }

    public class UpcomingEventService : BaseService<UpcomingEventDto, UpcomingEvent>, IUpcomingEventService
    {
        public UpcomingEventService(AppDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<long> Save(UpcomingEventDto record)
        {
            var vendor = await _context.Users.FirstOrDefaultAsync(q => q.UserName == record.Username);
            if (vendor == null) throw new Exception("Unknown Account");

            var (filename, extension) = new MediaHelper(record.RootPath).SaveDataMedia(record.Media);
            var status = new UpcomingEvent
            {
                DateTime = DateTime.UtcNow,
                Media = filename,
                MediaExtension = extension,
                Theme = record.Theme,
                VendorId = vendor.Id,
                Location = record.Location,
            };

            _context.UpcomingEvents.Add(status);
            _context.SaveChanges();

            return status.Id;
        }

        public async Task<List<UpcomingEventCardDto>> Query(UpcomingEventFilter filter)
        {
            var data = await filter.BuildQuery(_context.UpcomingEvents.Select(x => x))
                .Include(x => x.Vendor)
                .OrderByDescending(x => x.Id)
                .Skip(filter.Skip()).Take(filter.Size)
                .ToListAsync();

            return data.Select(x => new UpcomingEventCardDto
            {
                Id = x.Id,
                Vendor = x.Vendor.Name,
                Media = x.Media,
                MediaExtension = x.MediaExtension,
                Theme = x.Theme,
                Location = x.Location,
                DateTime = x.DateTime,
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(x.DateTime.Month),
                Number = x.DateTime.Day.ToString(),
                Time = x.DateTime.ToShortTimeString(),
                Day = x.DateTime.DayOfWeek.ToString()
            }).ToList();
        }

        public async Task<string> GetMediaExtension(string fileName)
        {
            var ext = await _context.UpcomingEvents.FirstOrDefaultAsync(q => q.Media == fileName);

            return ext.MediaExtension;
        }
    }
}
