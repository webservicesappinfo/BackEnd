﻿using EventBus.Abstractions;
using Globals.Sevices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Abstractions;
using UserService.Models;

namespace UserService.Services
{
    public class UserRepoService : RepoServiceBase<User, UserContext>, IUserRepoService
    {
        private readonly ILogger<UserRepoService> _logger;
        private readonly IEventBus _eventBus;

        public UserRepoService(ILogger<UserRepoService> logger, IEventBus eventBus) : base(logger)
        {
            _logger = logger;
            _eventBus = eventBus;
        }        

        public bool AddCompany(Guid uidfb, Guid company, String name)
        {
            using (var db = new UserContext())
            {
                var findUser = db.Values.Include(x => x.Companies).FirstOrDefault(x => x.UIDFB == uidfb);
                if (findUser == null) return false;
                if (findUser.Companies.Any(x => x.Guid == company)) return false;
                findUser.Companies.Add(new Globals.Models.CompanyRef<User>() { RefGuid = company, Name = name });
                db.SaveChanges();
            }
            return true;
        }

        public bool DelCompany(Guid uidfb, Guid company)
        {
            using (var db = new UserContext())
            {
                var findUser = db.Values.Include(x=>x.Companies).FirstOrDefault(x => x.UIDFB == uidfb);
                if (findUser == null) return false;
                var findCompany = findUser.Companies.FirstOrDefault(x => x.RefGuid == company);
                if (findCompany == null) return false;
                findUser.Companies.Remove(findCompany);
                db.SaveChanges();
            }
            return true;
        }

        public bool AddOffer(Guid guid, string name, Guid masterGuid)
        {
            using (var db = new UserContext())
            {
                var findUser = db.Values.Include(x=>x.Offers).FirstOrDefault(x => x.UIDFB == masterGuid);
                if (findUser == null) return false;
                if (findUser.Offers.Any(x => x.Guid == guid)) return false;
                findUser.Offers.Add(new Globals.Models.OfferRef<User>() { RefGuid = guid, Name = name });
                db.SaveChanges();
            }
            return true;
        }

        public bool OnDelOffer(Guid guid, Guid masterGuid)
        {
            using (var db = new UserContext())
            {
                var findUser = db.Values.Include(x=>x.Offers).FirstOrDefault(x => x.UIDFB == masterGuid);
                if (findUser == null) return false;
                var findoffer = findUser.Offers.FirstOrDefault(x => x.Guid == guid);
                if (findoffer == null) return false;
                findUser.Offers.Remove(findoffer);
                db.SaveChanges();
            }
            return true;
        }

        public User GetUserByUIDFB(Guid uidfb)
        {
            User user = null;
            using (var db = new UserContext())
                user = db.Values.FirstOrDefault(x => x.UIDFB == uidfb);
            return user;
        }
    }
}
