using EventBus.Abstractions;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Models;
using UserService.Protos;

namespace UserService.Services
{
    public class UserServiceImp : Protos.User.UserBase
    {
        private readonly ILogger<UserServiceImp> _logger;
        private readonly IEventBus _eventBus;

        public UserServiceImp(ILogger<UserServiceImp> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }
        public override Task<AddUserReply> AddUser(AddUserRequest request, ServerCallContext context)
        {
            var result = false;
            using (var users = new UserContext())
            {
                var requestGuid = new Guid(request.Guid);
                if (!users.Values.Any(x => x.Guid == requestGuid))
                {
                    var user = new Models.User { Guid = requestGuid, Name = request.Name, Token = request.Token };
                    users.Values.Add(user);
                    users.SaveChanges();
                    result = true;
                    //_eventBus.Publish(new AddUserEvent(user.Name, user.Guid, user.Token));
                }
            }
            return Task.FromResult(new AddUserReply { Result = result });
        }

        public override Task<GetUsersReply> GetUsers(GetUsersRequest request, ServerCallContext context)
        {
            var reply = new GetUsersReply();
            using (var users = new UserContext())
                foreach (var user in users.Values)
                    reply.Names.Add($"{user.Guid}:{user.Name}");
            return Task.FromResult(reply);
        }

        public override Task<GetUserReply> GetUser(GetUserRequest request, ServerCallContext context)
        {
            return base.GetUser(request, context);
        }

        public override Task<UpdateUserReply> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            return base.UpdateUser(request, context);
        }

        public override Task<RemoveUserReply> RemoveUser(RemoveUserRequest request, ServerCallContext context)
        {
            return base.RemoveUser(request, context);
        }
    }
}
