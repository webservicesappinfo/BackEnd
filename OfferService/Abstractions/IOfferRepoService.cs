using EventBus.Events.ServicesEvents.CompanyEvents;
using Globals.Abstractions;
using OfferService.Models;
using System;

namespace OfferService.Abstractions
{
    public interface IOfferRepoService : IRepoServiceBase<Offer> 
    {
        public bool SetStatus(Guid offerGuid, OfferStatus status);

        public bool SetInfoFromCompany(SendInfoForOffer @event);
    }
}
