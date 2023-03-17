using GirlyMerely.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlyMerely.Infrastructure.Repositories
{
    public class ProductMainFeaturesRepository : BaseRepository<ProductMainFeature, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ProductMainFeaturesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public ProductMainFeature GetByProductId(int productId)
        {
            ProductMainFeature productMainFeature = new ProductMainFeature();
            var mainFeatures = _context.ProductMainFeatures.Where(f => f.IsDeleted == false && f.ProductId == productId);
            foreach (var mf in mainFeatures)
            {
                if (mf.Quantity > 0)
                {
                    productMainFeature = mf;
                    break;
                }
            }

            if (productMainFeature.Quantity > 0) // there is a main feature that exist in supply
                return productMainFeature;

            return _context.ProductMainFeatures.FirstOrDefault(f => f.IsDeleted == false && f.ProductId == productId);
        }
        public ProductMainFeature GetByProductId(int productId,int mainFeatureId)
        {
            return _context.ProductMainFeatures.FirstOrDefault(f => f.IsDeleted == false && f.ProductId == productId && f.Id == mainFeatureId);
        }
        public List<ProductMainFeature> GetProductMainFeatures(int productId)
        {
            return _context.ProductMainFeatures.Include(f=>f.Feature).Include(f=>f.SubFeature).Where(f => f.IsDeleted == false && f.ProductId == productId).ToList();
        }


        public ProductMainFeature GetProductMainFeatureName(int productId,int mainFeatureId)
        {
            return _context.ProductMainFeatures.Include(f => f.Feature).Include(f => f.SubFeature).Where(f => f.IsDeleted == false && f.ProductId == productId && f.Id== mainFeatureId).FirstOrDefault();
        }
        
    }
}
