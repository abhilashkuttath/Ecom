using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    
    public class ProductsController: BaseApiController
    {
       
  
        private readonly IGenricRepository<Product> _productRepo;
        private readonly IGenricRepository<ProductBrand> _productBrandRepo;
        private readonly IGenricRepository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenricRepository<Product> productRepo,IGenricRepository<ProductBrand> productBrandRepo,
        IGenricRepository<ProductType> productTypeRepo,IMapper mapper)
        {
            _productTypeRepo = productTypeRepo;
            _productBrandRepo = productBrandRepo;
            _productRepo = productRepo;
            _mapper = mapper;
            
          
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams productParams)
        {

            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);
            var countSpec = new ProductWithFiltersForCountSpecification(productParams);
            var totalItem = await _productRepo.CountAsync(countSpec);
            
            var products = await _productRepo.ListAsync(spec);
            var data = _mapper
               .Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products);
            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex,productParams.PageSize,
            totalItem,data));
            
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec =new ProductsWithTypesAndBrandsSpecification(id);

            var product = await _productRepo.GetEntityWithSpec(spec);
            return _mapper.Map<Product,ProductToReturnDto>(product);
        }
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productBrandRepo.ListAllAsync());
        }
         [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _productTypeRepo.ListAllAsync());
        }
        
    }
}





//old implimentation
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class ProductsController: ControllerBase
//     {
//         private readonly IProductRepository _repo;

//         public ProductsController(IProductRepository repo)
//         {
//            _repo = repo;
//         }

//         [HttpGet]
//         public async Task<ActionResult<List<Product>>> GetProducts()
//         {
//             var products = await _repo.GetProductsAsync();
//             return Ok(products);
            
//         }
//         [HttpGet("{id}")]
//         public async Task<ActionResult<Product>> GetProduct(int id)
//         {
//             return await _repo.GetProductByIdAsync(id);
//         }
//         [HttpGet("brands")]
//         public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
//         {
//             return Ok(await _repo.GetProductBrandsAsync());
//         }
//          [HttpGet("types")]
//         public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
//         {
//             return Ok(await _repo.GetProductBrandsAsync());
//         }
        
        
    
//     }






//}