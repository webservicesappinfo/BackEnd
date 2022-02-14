using EventBus.Events.ServicesEvents.CompanyEvents;
using Globals.Abstractions;
using OfferService.Models;
using System;

namespace OfferService.Abstractions
{
    public interface IOfferRepoService : IRepoServiceBase<Offer> 
    {
        bool SetStatus(Guid offerGuid, OfferStatus status);

        bool SetInfoFromCompany(SendInfoForOffer @event);

        bool OnDelCompanyEH(Guid companyGuid);
    }
}
