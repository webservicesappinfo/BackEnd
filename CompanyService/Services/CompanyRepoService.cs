using CompanyService.Abstractions;
using CompanyService.Models;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using Globals.Models;
using Globals.Sevices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompanyService.Services
{
    public class CompanyRepoService : RepoServiceBase<Company, CompanyContext>, ICompanyRepoService
    {
        private readonly ILogger<CompanyRepoService> _logger;
        private readonly IEventBus _eventBus;

        public CompanyRepoService(ILogger<CompanyRepoService> logger, IEventBus eventBus) : base(logger)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public List<Company> GetCompaniesByOwner(Guid owner)
        {
            using (var db = new CompanyContext())
                return db.Values.Where(x=>x.OwnerGuid == owner).Include(x => x.Masters).Include(x => x.Offers).ToList();
        }

        public bool JoinToCompany(Guid guid, Guid masterGuid, String masterName)
        {
            using (var db = new CompanyContext())
            {
                var fitCompany = db.Values.Include(x=>x.Masters).FirstOrDefault(x=>x.Guid == guid);
                if (fitCompany == null) return false;
                if (fitCompany.Masters.Any(x => x.Guid == masterGuid)) return false;
                fitCompany.Masters.Add(new MasterRef<Company>() { RefGuid = masterGuid, Name = masterName });
                db.SaveChanges();
            }
            return true;
        }

        public List<Company> GetCompaniesByMaster(Guid master)
        {
            using (var db = new CompanyContext())
                return db.Values.Include(x => x.Masters).Include(x => x.Offers).Where(x => x.Masters.Any(m=>m.RefGuid == master)).ToList();
        }

        public bool DelMaster(Guid company, Guid master)
        {
            using (var db = new CompanyContext())
            {
                var fitCompany = db.Values.Include(x => x.Masters).FirstOrDefault(x => x.Guid == company);
                if (fitCompany == null) return false;
                var fitMaster = fitCompany.Masters.FirstOrDefault(x => x.RefGuid == master);
                if (fitMaster == null) return false;
                db.Masters.Remove(fitMaster);
                db.SaveChanges();
            }
            return true;
        }
    }
}
