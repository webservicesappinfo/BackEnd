using Globals.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Models;

namespace UserService.Abstractions
{
    public interface IUserRepoService : IRepoServiceBase<User>
    {
        public User GetUserByUIDFB(Guid uidfb);

        public Boolean AddCompany(Guid uidfb, Guid company, String name);

        public Boolean DelCompany(Guid uidfb, Guid company);

        public Boolean JoinToCompany(Guid uidfb, Guid company, String name);

        public Boolean UnJoinCompany(Guid uidfb, Guid company);

        public Boolean AddOffer(Guid guid, String Name, Guid masterGuid);

        public Boolean OnDelOffer(Guid guid, Guid masterGuid);
    }
}
