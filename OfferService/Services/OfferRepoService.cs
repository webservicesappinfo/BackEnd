using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.CompanyEvents;
using EventBus.Events.ServicesEvents.OfferEvents;
using Globals.Sevices;
using Microsoft.Extensions.Logging;
using OfferService.Abstractions;
using OfferService.Models;
using System;
using System.Linq;

namespace OfferService.Services
{
    public class OfferRepoService: RepoServiceBase<Offer, OfferContext>, IOfferRepoService
    {
        private readonly ILogger<OfferRepoService> _logger;
        private readonly IEventBus _eventBus;

        public OfferRepoService(ILogger<OfferRepoService> logger, IEventBus eventBus) : base(logger) { _logger = logger; _eventBus = eventBus; }

        public bool OnDelCompanyEH(Guid companyGuid)
        {
            using (var db = new OfferContext())
            {
                foreach(var offer in db.Values.Where(x => x.CompanyGuid == companyGuid))
                {
                    db.Remove(offer);
                    _eventBus.Publish(new DelOfferEvent(offer.Name, offer.Guid, offer.MasterGuid));
                }
                db.SaveChanges();
            }
            return true;
        }

        public bool SetInfoFromCompany(SendInfoForOffer @event)
        {
            using (var db = new OfferContext())
            {
                var offer = db.Values.FirstOrDefault(x => x.Guid == @event.OfferGuid);
                if (offer == null) return false;
                offer.CompanyName = @event.CompanyName;
                offer.Lat = @event.Lat;
                offer.Lng = @event.Lng;
                db.SaveChanges();
            }
            return true;
        }

        public bool SetStatus(Guid offerGuid, OfferStatus status)
        {
            using (var db = new OfferContext())
            {
                var offer = db.Values.FirstOrDefault(x=>x.Guid == offerGuid);
                if (offer == null) return false;
                offer.Status = status;
                db.SaveChanges();
            }
            return true;
        }
    }
}
