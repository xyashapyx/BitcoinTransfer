using AutoMapper;
using BitcoinTransfer.Models;
using BitcoinTransfer.ViewModels;

namespace BitcoinTransfer
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<TransactionModel, LastTransactionModel>();
        }
    }
}
