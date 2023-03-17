using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GirlyMerely.Core.Models;

namespace GirlyMerely.Infrastructure.Repositories
{
    public class OffersRepository : BaseRepository<Offer, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public OffersRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
        public List<Brand> GetOfferBrands(int offerId)
        {
            var brands = new List<Brand>();

            var discounts = _context.Discounts.Where(d => d.OfferId == offerId && d.IsDeleted == false).ToList();
            foreach(var discount in discounts)
            {
                if(discount.BrandId != null)
                {
                    var brand = _context.Brands.FirstOrDefault(b => b.Id == discount.BrandId && b.IsDeleted == false);
                    if (brand != null)
                        brands.Add(brand);
                }
            }

            return brands;
        }
        public List<ProductGroup> GetOfferProductGroups(int offerId)
        {
            var productGroups = new List<ProductGroup>();

            var discounts = _context.Discounts.Where(d => d.OfferId == offerId && d.IsDeleted == false).ToList();
            foreach (var discount in discounts)
            {
                if (discount.ProductGroupId != null)
                {
                    var productGroup = _context.ProductGroups.FirstOrDefault(pg => pg.Id == discount.ProductGroupId && pg.IsDeleted == false);
                    if (productGroup != null)
                        productGroups.Add(productGroup);
                }
            }

            return productGroups;
        }
    }
}
