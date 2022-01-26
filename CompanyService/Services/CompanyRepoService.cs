using CompanyService.Abstractions;
using CompanyService.Models;
using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using EventBus.Events.ServicesEvents.OfferEvents;
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
                //return db.Values.Where(x=>x.OwnerGuid == owner).Include(x => x.Workers).Include(x => x.Offers).ToList();
                return db.Values.Where(x => x.OwnerGuid == owner).Include(x => x.Workers).ToList();
        }

        public bool JoinToCompany(Guid guid, Guid masterGuid, String masterName)
        {
            using (var db = new CompanyContext())
            {
                var fitCompany = db.Values.Include(x=>x.Workers).FirstOrDefault(x=>x.Guid == guid);
                if (fitCompany == null) return false;
                if (fitCompany.Workers.Any(x => x.Guid == masterGuid)) return false;
                fitCompany.Workers.Add(new Worker() { RefGuid = masterGuid, Name = masterName });
                db.SaveChanges();
            }
            return true;
        }

        public List<Company> GetCompaniesByWorker(Guid master)
        {
            using (var db = new CompanyContext())
                //return db.Values.Include(x => x.Workers).Include(x => x.Offers).Where(x => x.Masters.Any(m=>m.RefGuid == master)).ToList();
                return db.Values.Include(x => x.Workers).Where(x => x.Workers.Any(m => m.RefGuid == master)).ToList();
        }

        public Boolean DelWorker(Guid company, Guid master)
        {
            using (var db = new CompanyContext())
            {
                var fitCompany = db.Values.Include(x => x.Workers).FirstOrDefault(x => x.Guid == company);
                if (fitCompany == null) return false;
                var fitMaster = fitCompany.Workers.FirstOrDefault(x => x.RefGuid == master);
                if (fitMaster == null) return false;
                db.Workers.Remove(fitMaster);
                db.SaveChanges();
            }
            return true;
        }

        public Boolean SetCompanyLocation(Guid companyGuid, Double? lat, Double? lng)
        {
            using (var db = new CompanyContext())
            {
                var company = db.Values.FirstOrDefault(x => x.Guid == companyGuid);
                if(company == null) return false;
                company.Lat = lat;
                company.Lng = lng;
                db.SaveChanges();
            }
            return true;
        }

        public Boolean OnAddOfferEvent(AddOfferEvent @event)
        {
            using (var db = new CompanyContext())
            {
                var fitCompany = db.Values.Include(x => x.Workers).FirstOrDefault(x => x.Guid == @event.CompanyGuid);
                if (fitCompany == null) return false;
                var fitWorker = fitCompany.Workers.FirstOrDefault(x => x.RefGuid == @event.MasterGuid);
                if (fitWorker == null) return false;
                fitWorker.WorkerOffers.Add(new WorkerOffer() { RefGuid = @event.Guid, Name = @event.Name });
                db.SaveChanges();
            }
            return true;
        }
    }
}
