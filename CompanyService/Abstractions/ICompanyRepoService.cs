using CompanyService.Models;
using EventBus.Events.ServicesEvents.OfferEvents;
using Globals.Abstractions;
using System;
using System.Collections.Generic;

namespace CompanyService.Abstractions
{
    public interface ICompanyRepoService: IRepoServiceBase<Company>
    {
        List<Company> GetCompaniesByOwner(Guid owner);

        List<Company> GetCompaniesByWorker(Guid master);

        Boolean JoinToCompany(Guid guid, Guid masterGuid, String masterName);

        Boolean DelWorker(Guid company, Guid master);

        Boolean SetCompanyLocation(Guid companyGuid, Double? lat, Double? lng);

        Boolean OnAddOfferEvent(AddOfferEvent @event);
    }
}
