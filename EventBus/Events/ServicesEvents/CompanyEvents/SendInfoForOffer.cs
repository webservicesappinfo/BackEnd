using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events.ServicesEvents.CompanyEvents
{
    public class SendInfoForOffer : IntegrationEvent
    {
        public Guid OfferGuid { get; private set; }
        public String CompanyName { get; private set; }
        public Double Lat { get; private set; }
        public Double Lng { get; private set; }

        public SendInfoForOffer(Guid offerGuid, string companyName, double lat, double lng)
        {
            OfferGuid = offerGuid;
            CompanyName = companyName;
            Lat = lat;
            Lng = lng;
        }
    }
}
