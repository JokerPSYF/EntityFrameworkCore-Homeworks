using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.Models;

namespace CarDealer
{
    using CarDealer.DTO.Cars;
    using CarDealer.DTO.Parts;
    using CarDealer.DTO.Suppliers;
    using CarDealer.DTO.Customers;
    using CarDealer.DTO.Sales;

    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //querry 9
            this.CreateMap<ImportSuppliersDto, Supplier>();

            //querry 10
            this.CreateMap<ImportPartDTO, Part>();

            //querry 12
            this.CreateMap<ImportCustomersDTO, Customer>();

            //querry 13
            this.CreateMap<ImportSaleDTO, Sale>();
            
        }
    }
}
