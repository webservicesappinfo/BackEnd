using Globals.Sevices;
using Microsoft.Extensions.Logging;
using OfferService.Abstractions;
using OfferService.Models;
using System;

namespace OfferService.Services
{
    public class OfferRepoService: RepoServiceBase<Offer, OfferContext>, IOfferRepoService
    {
        private readonly ILogger<OfferRepoService> _logger;
        public OfferRepoService(ILogger<OfferRepoService> logger) : base(logger) { _logger = logger; }

        public bool SetStatus(Guid offerGuid, OfferStatus status)
        {
            var offer = GetEntity(offerGuid);
            if(offer == null) return false;
            offer.Status = status;
            return true;
        }
    }
}
