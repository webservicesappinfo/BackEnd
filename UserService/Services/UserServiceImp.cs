using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.UserRepoEvents;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Abstractions;
using UserService.Models;
using UserService.Protos;

namespace UserService.Services
{
    public class UserServiceImp : Protos.User.UserBase
    {
        private readonly ILogger<UserServiceImp> _logger;
        private readonly IEventBus _eventBus;
        private readonly IUserRepoService _userRepoService;

        public UserServiceImp(ILogger<UserServiceImp> logger, IEventBus eventBus, IUserRepoService userRepoService)
        {
            _logger = logger;
            _eventBus = eventBus;
            _userRepoService = userRepoService;
        }
        public override Task<AddUserReply> AddUser(AddUserRequest request, ServerCallContext context)
        {
            var user = new Models.User() 
            { 
                Name = request.Name, 
                UIDFB = new Guid(request.UidFB), 
                Token = request.Token 
            };
            var result = _userRepoService.AddUser(user);
            if(result)
                _eventBus.Publish(new AddUserEvent(user.Name, user.Guid.ToString(), user.Token));
            return Task.FromResult(new AddUserReply { Result = result });
        }

        public override Task<GetUsersReply> GetUsers(GetUsersRequest request, ServerCallContext context)
        {
            var reply = new GetUsersReply();
            var users = _userRepoService.GetAllUsers();
            foreach (var user in users)
                reply.Names.Add($"{user.UIDFB}:{user.Name}");
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

        public override Task<DelUserReply> DelUser(DelUserRequest request, ServerCallContext context)
        {
            var user = _userRepoService.GetUserByUIDFB(new Guid(request.UidFB));
            var reply = new DelUserReply();
            if (user == null) return Task.FromResult(reply);

            var result = _userRepoService.DelUser(user);
            if(result)
                _eventBus.Publish(new DelUserEvent(user.Name, user.UIDFB.ToString(), user.Token));

            return Task.FromResult(reply);
        }
    }
}
