using AutoMapper;
using NeuralNetworkManager.Models;
using WPFDesktopUI.Models;

namespace WPFDesktopUI.Mappers
{
    class DataMapper
    {
        public static IMapper Default { get; } = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<BaseModel, BaseDataModel>();
            cfg.CreateMap<BaseDataModel, BaseModel>();
            cfg.CreateMap<BaseDataModel, BaseDataModel>();

        }).CreateMapper();
    }
}
