using Globals.Sevices;
using Microsoft.Extensions.Logging;
using OfferService.Abstractions;
using OfferService.Models;

namespace OfferService.Services
{
    public class OfferRepoService: RepoServiceBase<Offer, OfferContext>, IOfferRepoService
    {
        public OfferRepoService(ILogger<RepoServiceBase<Offer, OfferContext>> logger) : base(logger) { }
    }
}
