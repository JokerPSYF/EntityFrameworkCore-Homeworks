using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.Models;

namespace CarDealer
{
    using DTO;
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSuppliersDto, Supplier>();
        
        }
    }
}
