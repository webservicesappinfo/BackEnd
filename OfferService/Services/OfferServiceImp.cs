using EventBus.Abstractions;
using EventBus.Events.ServicesEvents.OfferEvents;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using OfferService.Abstractions;
using OfferService.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileApiGetway;
using OfferService.Models;
using Offer = OfferService.Protos.Offer;

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
            var offer = new Models.Offer()
            {
                Name = request.Offer.Name,
                CompanyGuid = new Guid(request.Offer.CompanyGuid),
                CompanyName = request.Offer.CompanyName,
                MasterGuid = new Guid(request.Offer.MasterGuid),
                MasterName = request.Offer.MasterName,
                SkillGuid = new Guid(request.Offer.SkillGuid),
                SkillName = request.Offer.SkillName
            };
            var result = _offerRepoService.AddEntity(offer);

            if (result)
                _eventBus.Publish(new AddOfferEvent(offer.Name, offer.Guid, offer.CompanyGuid, offer.MasterGuid));

            return Task.FromResult(new AddOfferReply { Result = result });
        }
        

        public override Task<GetOfferReply> GetOffer(GetOfferRequest request, ServerCallContext context)
        {
            var offer = _offerRepoService.GetEntity(new Guid(request.Guid));
            if (offer == null) return Task.FromResult(new GetOfferReply() { Result = false });
            return Task.FromResult(new GetOfferReply()
            {
                Result = true,
                Offer = ConvertOffer(offer)
            });
        }

        public override Task<GetOffersReply> GetOffers(GetOffersRequest request, ServerCallContext context)
        {
            var reply = new GetOffersReply();
            foreach(var guid in request.Guids)
            {
                var offer = _offerRepoService.GetEntity(new Guid(guid));
                if (offer == null) continue;
                reply.Offers.Add(ConvertOffer(offer));
            }
            return Task.FromResult(reply);
        }

        public override Task<DelOfferReply> DelOffer(DelOfferRequest request, ServerCallContext context)
        {
            var offer = request.Offer;
            var result = _offerRepoService.DelEntity(new Guid(offer.Guid));
            if (result)
                _eventBus.Publish(new DelOfferEvent(offer.Name, new Guid(offer.CompanyGuid), new Guid(offer.MasterGuid)));
            return Task.FromResult(new DelOfferReply() { Result = result});
        }
        private OfferApi ConvertOffer(Models.Offer offer)
        => new OfferApi()
        {
            Name = offer.Name,
            Guid = offer.Guid.ToString(),
            Desc = offer.Description,
                CompanyGuid = offer.CompanyGuid.ToString(),
                CompanyName = offer.CompanyName,
                MasterGuid = offer.MasterGuid.ToString(),
                MasterName = offer.MasterName,
                SkillGuid = offer.SkillGuid.ToString(),
                SkillName = offer.SkillName,
                OrderGuid = offer.OrderGuid.ToString(),
                Status = offer.Status.ToString()
            };
    }
}
