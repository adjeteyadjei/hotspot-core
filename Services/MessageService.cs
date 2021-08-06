using AutoMapper;
using Hotvenues.Data;
using Hotvenues.Integrations;
using Hotvenues.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotvenues.Services
{
    public class MessageDto
    {
        public long Id { get; set; }
        public string Recipient { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public MessageStatus Status { get; set; }
        public MessageType Type { get; set; }
        public string Response { get; set; }
        public DateTime TimeStamp { get; set; }
        public byte[] Attachment { get; set; }
        public string FileName { get; set; }
    }

    public class ContactDto
    {
        public string Id { get; set; }
        public long RecordId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public ContactType Type { get; set; }
        public string Info { get; set; }
    }

    public interface IMessageService : IModelService<MessageDto>
    {
        Task<List<MessageDto>> Query(MessageFilter filter);
        Task<List<ContactDto>> SearchContacts(string term);
        Task<Tuple<bool, string>> Send(MessageDto msg);
        Task<Tuple<bool, string>> Resend(long id);
        Task<Tuple<bool, string>> BulkSend(BulkMessage msg);
        Task<decimal> CreditBalance();
        Task ProcessOutwardMessages();
        Task<string> GetTopUpLink();
    }

    public class BulkMessage
    {
        public string Subject { get; set; }
        public string Text { get; set; }
        public MessageType Type { get; set; }
        public virtual List<ContactDto> Contacts { get; set; }
    }

    public class MessageFilter
    {
        [FromQuery]
        public string Name { get; set; }
        [FromQuery]
        public string Recipient { get; set; }
        [FromQuery]
        public MessageStatus? Status { get; set; }
        [FromQuery]
        public MessageType? Type { get; set; }
        [FromQuery]
        public DateTime? StartDate { get; set; }
        [FromQuery]
        public DateTime? EndDate { get; set; }
        [FromQuery]
        public bool? Scheduled { get; set; }
        [FromQuery]
        public long AccountId { get; set; }

        [FromQuery(Name = "_page")]
        public int Page { get; set; }

        [FromQuery(Name = "_size")]
        public int Size { get; set; }

        public int Skip() { return (Page - 1) * Size; }

        public IQueryable<Message> BuildQuery(IQueryable<Message> query)
        {
            if (!string.IsNullOrWhiteSpace(Name)) query = query.Where(x => x.Name.ToLower().Contains(Name.ToLower()));
            if (!string.IsNullOrWhiteSpace(Recipient)) query = query.Where(x => x.Recipient.ToLower().Contains(Recipient.ToLower()));
            if (Status.HasValue) query = query.Where(x => x.Status == Status);
            if (Type.HasValue) query = query.Where(x => x.Type == Type);
            if (StartDate.HasValue) query = query.Where(q => q.TimeStamp >= StartDate.Value);
            if (EndDate.HasValue)
            {
                EndDate = EndDate.Value.AddHours(24);
                query = query.Where(q => q.TimeStamp <= EndDate.Value);
            }
            return query;
        }

    }

    public enum ContactType
    {
        All,
        Patient,
        PayGroup,
        PatientClass,
        Occupation,
        Town,
        Country
    }

    public class MessageService : BaseService<MessageDto, Message>, IMessageService
    {
        public MessageService(AppDbContext context, IMapper mapper) : base(context, mapper) { }

        public async Task<List<MessageDto>> Query(MessageFilter filter)
        {
            var data = filter.BuildQuery(_context.Messages.Select(x => x))
                .OrderByDescending(x => x.Id)
                .Skip(filter.Skip()).Take(filter.Size).ToList();

            return await Task.FromResult(data.Select(x => new MessageDto
            {
                Id = x.Id,
                Name = x.Name,
                TimeStamp = x.TimeStamp,
                Response = x.Response,
                Subject = x.Subject,
                Recipient = x.Recipient,
                Status = x.Status,
                Text = x.Text,
                Type = x.Type
            }).ToList());
        }

        public async Task<List<ContactDto>> SearchContacts(string term)
        {
            var contacts = new List<ContactDto>();
            if (string.IsNullOrEmpty(term)) return contacts;

            /*contacts.AddRange(_context.Patients.Where(q => q.Name.Contains(term)).Take(5).Select(x => new ContactDto
            {
                Id = x.Id + " " + ContactType.Patient,
                RecordId = x.Id,
                Name = x.Name,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Type = ContactType.Patient,
                Info = ContactType.Patient.ToString()
            }));


            contacts.AddRange(_context.PayGroups.Where(q => q.Name.Contains(term)).Take(5).Select(x => new ContactDto
            {
                Id = x.Id + " " + ContactType.PayGroup,
                RecordId = x.Id,
                Name = x.Name,
                PhoneNumber = "",
                Email = x.Email,
                Type = ContactType.PayGroup,
                Info = ContactType.PayGroup.ToString()
            }));

            if (term.ToUpper().Contains("ALL"))
            {
                contacts.Add(new ContactDto
                {
                    Id = 0 + " " + ContactType.All,
                    RecordId = 0L,
                    Name = "All Patients",
                    PhoneNumber = "",
                    Email = "",
                    Type = ContactType.All,
                    Info = "All Patients"
                });
            }*/

            return await Task.FromResult(contacts);
        }

        public async Task<Tuple<bool, string>> Send(MessageDto msg)
        {
            if (string.IsNullOrEmpty(msg.Recipient)) throw new Exception("No recipient provided");
            var apiKey = _context.AppSettings.FirstOrDefault(q => q.Name == ConfigKeys.ApiKey)?.Value;
            var sender = _context.AppSettings.FirstOrDefault(q => q.Name == ConfigKeys.SenderName)?.Value;

            var postman = new Postman(apiKey, sender);

            var (success, response) = msg.Type == MessageType.SMS
            ? await postman.SendMessage(msg.Recipient, msg.Text)
            : await postman.SendMessage(msg.Recipient, msg.Subject, msg.Text, msg.Attachment, msg.FileName);

            var message = new Message
            {
                Name = msg.Name,
                Recipient = msg.Recipient,
                Status = success ? MessageStatus.Sent : MessageStatus.Failed,
                Subject = msg.Subject,
                Response = response,
                TimeStamp = DateTime.UtcNow,
                Text = msg.Text,
                Type = msg.Type
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return new Tuple<bool, string>(success, response);

        }

        public async Task<Tuple<bool, string>> Resend(long id)
        {
            var message = _context.Messages.Find(id);
            if (string.IsNullOrEmpty(message.Recipient)) throw new Exception("No recipient provided");
            var apiKey = _context.AppSettings.FirstOrDefault(q => q.Name == ConfigKeys.ApiKey)?.Value;
            var sender = _context.AppSettings.FirstOrDefault(q => q.Name == ConfigKeys.SenderName)?.Value;

            var postman = new Postman(apiKey, sender);

            var (success, response) = message.Type == MessageType.SMS
                ? await postman.SendMessage(message.Recipient, message.Text)
                : await postman.SendMessage(message.Recipient, message.Subject, message.Text);

            message.Status = success ? MessageStatus.Sent : MessageStatus.Failed;

            _context.Messages.Update(message);
            _context.SaveChanges();

            return new Tuple<bool, string>(success, response);
        }

        public async Task<Tuple<bool, string>> BulkSend(BulkMessage msg)
        {
            /*var message = RefactorMessage(msg);
            if (string.IsNullOrEmpty(message.Recipient)) throw new Exception("No recipient provided");
            var apiKey = _context.AppSettings.FirstOrDefault(q => q.Name == ConfigKeys.ApiKey)?.Value;
            var sender = _context.AppSettings.FirstOrDefault(q => q.Name == ConfigKeys.SenderName)?.Value;

            var postman = new Postman(apiKey, sender);

            var (success, response) = message.Type == MessageType.SMS
                ? await postman.SendMessage(message.Recipient, message.Text)
                : await postman.SendMessage(message.Recipient, message.Subject, message.Text);

            message.TimeStamp = DateTime.UtcNow;
            message.Status = success ? MessageStatus.Sent : MessageStatus.Failed;
            message.Response = response;

            _context.Messages.Add(message);
            _context.SaveChanges();
            return new Tuple<bool, string>(success, response);*/
						return await Task.FromResult(new Tuple<bool, string>(false, "Not Implemented"));
        }

        public async Task<decimal> CreditBalance()
        {
            try
            {
                var apiKey = _context.AppSettings.FirstOrDefault(q => q.Name == ConfigKeys.ApiKey)?.Value;
                var sender = _context.AppSettings.FirstOrDefault(q => q.Name == ConfigKeys.SenderName)?.Value;

                var postman = new Postman(apiKey, sender);
                return await postman.CreditBalance();
            }
            catch (Exception) { return 0; }
        }

        public async Task ProcessOutwardMessages()
        {
            //Fetch MessageOutwards With Status 
            var msgOutwards = _context.MessageOutwards.Where(q => !q.Processed).ToList();
            if (!msgOutwards.Any()) return;

            var apiKey = _context.AppSettings.FirstOrDefault(q => q.Name == ConfigKeys.ApiKey)?.Value;
            var sender = _context.AppSettings.FirstOrDefault(q => q.Name == ConfigKeys.SenderName)?.Value;
            var postman = new Postman(apiKey, sender);

            //Process The Messages [Send SMS or Email base on message type]
            foreach (var msg in msgOutwards)
            {
                if (string.IsNullOrEmpty(msg.Recipient)) continue;

                var (success, response) = msg.Type == MessageType.SMS
                    ? await postman.SendMessage(msg.Recipient, msg.Text)
                    : await postman.SendMessage(msg.Recipient, msg.Subject, msg.Text);

                //Save Processed Message
                _context.Messages.Add(new Message
                {
                    Name = msg.Name,
                    Recipient = msg.Recipient,
                    Status = success ? MessageStatus.Sent : MessageStatus.Failed,
                    Subject = msg.Subject,
                    Response = response,
                    TimeStamp = DateTime.UtcNow,
                    Text = msg.Text,
                    Type = msg.Type
                });

                //Mark Outward Message as Processed
                msg.Processed = true;
                _context.MessageOutwards.Update(msg);
            }

            _context.SaveChanges();

        }

        private Message RefactorMessage(BulkMessage msg)
        {
            var message = new Message { Subject = msg.Subject, Text = msg.Text, Type = msg.Type, Recipient = "" };

            /*foreach (var contact in msg.Contacts)
            {
                switch (contact.Type)
                {
                    case ContactType.All:
                        message.Recipient += GetContacts(_context.Patients.ToList(), msg.Type);
                        message.Name = "All Patients";
                        break;
                    case ContactType.Patient:
                        var patient = _context.Patients.Find(contact.RecordId);
                        if (msg.Type == MessageType.SMS && string.IsNullOrEmpty(patient.PhoneNumber) || msg.Type == MessageType.Email && string.IsNullOrEmpty(patient.Email)) { continue; }
                        message.Recipient += msg.Type == MessageType.SMS ? patient.PhoneNumber : patient.Email + ",";
                        message.Name += $"{patient.Name}, ";
                        break;
                    case ContactType.PayGroup:
                        message.Recipient = GetContacts(_context.Patients.Where(q => q.Accounts.Any(a => a.PayGroupId == contact.RecordId)).ToList(), msg.Type);
                        message.Name = contact.Name.Trim();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }*/

            return message;
        }

        /*private static string GetContacts(IReadOnlyCollection<Patient> members, MessageType type)
        {
            var phoneNumbers = string.Empty;
            var emails = string.Empty;
            members.Where(x => !string.IsNullOrEmpty(x.PhoneNumber)).ToList().ForEach(m => phoneNumbers += m.PhoneNumber + ",");
            members.Where(x => !string.IsNullOrEmpty(x.Email)).ToList().ForEach(m => emails += m.Email + ",");
            return (type == MessageType.SMS) ? phoneNumbers : emails;
        }*/

        public async Task<string> GetTopUpLink()
        {
            var apiKey = await _context.AppSettings.FirstOrDefaultAsync(q => q.Name == ConfigKeys.ApiKey);
            var url = $"https://dnpost.herokuapp.com/#/topuprequest/{apiKey.Value}";
            return url;
        }
    }

}