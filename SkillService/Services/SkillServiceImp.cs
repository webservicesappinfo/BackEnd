using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.SkillEvents;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using SkillService.Abstractions;
using SkillService.Protos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkillService.Services
{
    public class SkillServiceImp : Skill.SkillBase
    {
        private readonly ILogger<SkillServiceImp> _logger;
        private readonly IEventBus _eventBus;
        private readonly ISkillRepoService _skillRepoService;

        public SkillServiceImp(ILogger<SkillServiceImp> logger, IEventBus eventBus, ISkillRepoService skillRepoService)
        {
            _logger = logger;
            _eventBus = eventBus;
            _skillRepoService = skillRepoService;
        }

        public override Task<AddSkillReply> AddSkill(AddSkillRequest request, ServerCallContext context)
        {
            var skill = new Models.Skill() { Name = request.Name, Description = request.Desc };
            var result = _skillRepoService.AddEntity(skill);
            if (result)
                _eventBus.Publish(new AddSkillEvent(skill.Name, skill.Guid));
            return Task.FromResult(new AddSkillReply { Result = result });
        }

        public override Task<GetSkillsReply> GetSkills(GetSkillsRequest request, ServerCallContext context)
        {
            var skills = _skillRepoService.GetEntities();
            var reply = new GetSkillsReply();

            foreach (var skill in skills)
            {
                reply.Guids.Add(skill.Guid.ToString());
                reply.Names.Add(skill.Name);
                reply.Desc.Add(skill.Description);
            }
            return Task.FromResult(reply);
        }

        public override Task<GetSkillReply> GetSkill(GetSkillRequest request, ServerCallContext context)
        {
            return base.GetSkill(request, context);
        }

        public override Task<UpdateSkillReply> UpdateSkill(UpdateSkillRequest request, ServerCallContext context)
        {
            return base.UpdateSkill(request, context);
        }

        public override Task<DelSkillReply> DelSkill(DelSkillRequest request, ServerCallContext context)
        {
            var skillGuid = new Guid(request.Guid);
            var result = _skillRepoService.DelEntity(skillGuid);
            if (result)
                _eventBus.Publish(new DelSkillEvent(skillGuid));
            return Task.FromResult(new DelSkillReply { Result = result });
        }
    }
}
