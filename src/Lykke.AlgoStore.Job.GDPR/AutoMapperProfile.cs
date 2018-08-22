using AutoMapper;
using Lykke.AlgoStore.Job.GDPR.AzureRepositories.Entities;
using Lykke.AlgoStore.Job.GDPR.Core.Domain.Entities;
using Lykke.AzureStorage.Tables;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.Job.GDPR
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //To entities
            CreateMap<SubscriberData, SubscriberEntity>();

            ForAllMaps((map, cfg) =>
            {
                if (map.DestinationType.IsSubclassOf(typeof(TableEntity))
                    || map.DestinationType.IsSubclassOf(typeof(AzureTableEntity)))
                {
                    cfg.ForMember("ETag", opt => opt.Ignore());
                    cfg.ForMember("PartitionKey", opt => opt.Ignore());
                    cfg.ForMember("RowKey", opt => opt.Ignore());
                    cfg.ForMember("Timestamp", opt => opt.Ignore());
                }
            });

            //From entities
            CreateMap<SubscriberEntity, SubscriberData>()
                .ForMember(src => src.ClientId, opt => opt.MapFrom(src => src.RowKey));
        }
    }
}
