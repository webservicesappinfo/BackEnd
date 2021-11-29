using Globals.Sevices;
using Microsoft.Extensions.Logging;
using SkillService.Abstractions;
using SkillService.Models;

namespace SkillService.Services
{
    public class SkillRepoService : RepoServiceBase<Skill, SkillContext>, ISkillRepoService
    {
        public SkillRepoService(ILogger<RepoServiceBase<Skill, SkillContext>> logger) : base(logger) { }
    }
}
