using CompanyService.Abstractions;
using CompanyService.Models;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompanyService.Services
{
    public class CompanyRepoService : ICompanyRepoService
    {
        private readonly ILogger<CompanyRepoService> _logger;
        private readonly IEventBus _eventBus;

        public CompanyRepoService(ILogger<CompanyRepoService> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public bool AddEntity(Company entity)
        {
            var result = false;
            using (var companies = new CompanyContext())
            {
                companies.Values.Add(entity);
                companies.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool DelEntity(Guid guid)
        {
            using (var db = new CompanyContext())
            {
                var findCompany = db.Values.Include(x => x.Masters).Include(x => x.Offers).FirstOrDefault(x => x.Guid == guid);
                if (findCompany == null) return false;

                foreach (var m in findCompany.Masters)
                    db.Masters.Remove(m);
                db.SaveChanges();

                foreach (var o in findCompany.Offers)
                    db.Offers.Remove(o);
                db.SaveChanges();

                db.Values.Remove(findCompany);
                db.SaveChanges();
            }
            return true;
        }

        public List<Company> GetEntities()
        {
            using (var db = new CompanyContext())
                return db.Values.Include(x => x.Masters).Include(x => x.Offers).ToList();
        }

        public Company GetEntity(Guid guid)
        {
            using (var db = new CompanyContext())
                return db.Values.Include(x => x.Masters).Include(x => x.Offers).FirstOrDefault(x => x.Guid == guid);
        }

        public bool UpdateEntity(Company company)
        {
            throw new NotImplementedException();
        }

        public List<Company> GetCompaniesByOwner(Guid owner)
        {
            using (var db = new CompanyContext())
                return db.Values.Where(x=>x.User == owner).Include(x => x.Masters).Include(x => x.Offers).ToList();
        }

        public bool JoinToCompany(Guid guid, Guid masterGuid)
        {
            using (var db = new CompanyContext())
            {
                var fitCompany = db.Values.Include(x=>x.Masters).FirstOrDefault(x=>x.Guid == guid);
                if (fitCompany == null) return false;
                if (fitCompany.Masters.Any(x => x.Guid == masterGuid)) return false;
                fitCompany.Masters.Add(new Globals.Models.MasterRef() { User = masterGuid });
                db.SaveChanges();
            }
            return true;
        }

        public List<Company> GetCompaniesByMaster(Guid master)
        {
            using (var db = new CompanyContext())
                return db.Values.Include(x => x.Masters).Include(x => x.Offers).Where(x => x.Masters.Any(m=>m.User == master)).ToList();
        }

        public bool DelMaster(Guid company, Guid master)
        {
            using (var db = new CompanyContext())
            {
                var fitCompany = db.Values.Include(x => x.Masters).FirstOrDefault(x => x.Guid == company);
                if (fitCompany == null) return false;
                var fitMaster = fitCompany.Masters.FirstOrDefault(x => x.User == master);
                if (fitMaster == null) return false;
                db.Masters.Remove(fitMaster);
                db.SaveChanges();
            }
            return true;
        }
    }
}
