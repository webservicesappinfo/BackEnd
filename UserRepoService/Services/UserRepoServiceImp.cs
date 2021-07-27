using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using EventBus.ServicesEvents.MobileClientEvents;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserRepoService.Models;

namespace UserRepoService
{
    public class UserRepoServiceImp : UserRepo.UserRepoBase
    {
        private readonly ILogger<UserRepoServiceImp> _logger;
        private readonly IEventBus _eventBus;

        public UserRepoServiceImp(ILogger<UserRepoServiceImp> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public override Task<GetUserReply> GetUser(GetUserRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetUserReply { Guid = "Hello " + request.Guid });
        }

        public override Task<AddUserReply> AddUser(AddUserRequest request, ServerCallContext context)
        {
            var isAdd = false;
            using (var db = new UserRepoContext())
            {
                if (!db.Users.Any(x => x.Guid == request.Guid))
                {
                    var user = new User { Guid = request.Guid, Name = request.Name, Token = request.Token };
                    db.Users.Add(user);
                    db.SaveChanges();
                    isAdd = true;
                    _eventBus.Publish(new AddUserEvent(user.Name, user.Guid, user.Token));
                }
            }
            return Task.FromResult(new AddUserReply { IsAdded = isAdd });
        }
        public override Task<GetUsersReply> GetUsers(GetUsersRequest request, ServerCallContext context)
        {
            var reply = new GetUsersReply();
            using (var db = new UserRepoContext())
                foreach (var user in db.Users)
                    reply.Names.Add($"{user.Guid}:{user.Name}");
            return Task.FromResult(reply);
        }

        public override Task<GetLastMessagesReply> GetLastMessage(GetLastMessageRequest request, ServerCallContext context)
        {
            /*using (var db = new UserRepoContext())
            {
                var reply = new GetLastMessagesReply();
                var findUser = db.Users.FirstOrDefault(x => x.Guid == request.Guid);
                if (findUser?.LastGetMessage == null) return Task.FromResult(reply);
                var msgParts = findUser.LastGetMessage.Split(':').ToList();
                if (msgParts.Count < 2) return Task.FromResult(reply);
                reply.ForGuid = msgParts[0];
                reply.Msg = msgParts[1];
                return Task.FromResult(reply);
            }*/
            var reply = new GetLastMessagesReply();
            return Task.FromResult(reply);
        }

        /*public override Task<SendMessageReply> SendMessage(SendMessageRequest request, ServerCallContext context)
        {
            //_eventService.Enqueue("NewMsg");
            var testEvent = new TestBusEvent() { Msg = "test" };
            if (_eventBus.Publish(testEvent))
            {

            }

            using (var db = new ApplicationContext())
            {
                var reply = new SendMessageReply() { Status = false };
                var forUser = db.Users.FirstOrDefault(x => x.Guid == request.ForGuid);
                var fromUser = db.Users.FirstOrDefault(x => x.Guid == request.FromGuid);
                if (forUser == null || fromUser == null) return Task.FromResult(reply);
                forUser.LastGetMessage = $"{fromUser}:{request.Msg}";
                db.SaveChanges();
                reply.Status = true;

                //_messaging.SendNotification(forUser.Token, "NewMsg", request.Msg);

                return Task.FromResult(reply);
            }
        }*/
    }
}
