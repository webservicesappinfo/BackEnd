using EventBus.Abstractions;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using OfferService.Abstractions;
using OfferService.Protos;
using System;
using System.Threading.Tasks;

namespace OfferService.Services
{
    public class OfferServiceImp : Offer.OfferBase
    {
        private readonly ILogger<OfferServiceImp> _logger;
        private readonly IEventBus _eventBus;
        private readonly IOfferRepoService _offerRepoService;

        public OfferServiceImp(ILogger<OfferServiceImp> logger, IEventBus eventBus, IOfferRepoService offerRepoService)
        {
            _logger = logger;
            _eventBus = eventBus;
            _offerRepoService = offerRepoService;
        }

        public override Task<AddOfferReply> AddOffer(AddOfferRequest request, ServerCallContext context)
        {
            var offer = new Models.Offer() { Name = request.Name, MasterName = request.MasterName, SkillName = request.SkillName };
            var result = _offerRepoService.AddEntity(offer);
            /*if (result)
                _eventBus.Publish(new AddSkillEvent(company.Name, company.Guid, company.User));*/
            return Task.FromResult(new AddOfferReply { Result = result });
        }

        public override Task<GetOfferReply> GetOffer(GetOfferRequest request, ServerCallContext context)
        {
            return base.GetOffer(request, context);
        }

        public override Task<GetOffersReply> GetOffers(GetOffersRequest request, ServerCallContext context)
        {
            var offers = _offerRepoService.GetEntities();
            var reply = new GetOffersReply();

            foreach (var offer in offers)
            {
                reply.Guids.Add(offer.Guid.ToString());
                reply.Names.Add(offer.Name);
                reply.MasterNames.Add(offer.MasterName);
            }
            return Task.FromResult(reply);
        }

        public override Task<UpdateOfferReply> UpdateOffer(UpdateOfferRequest request, ServerCallContext context)
        {
            return base.UpdateOffer(request, context);
        }

        public override Task<DelOfferReply> DelOffer(DelOfferRequest request, ServerCallContext context)
        {
            var result = _offerRepoService.DelEntity(new Guid(request.Guid));
            return Task.FromResult(new DelOfferReply { Result = result });
        }
    }
}
