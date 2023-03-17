using GirlyMerely.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlyMerely.Infrastructure.Repositories
{
    public class ShippingRepository : BaseRepository<ShippingPrice, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ShippingRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }


        public ShippingPrice GetCorrespondingShippingPrice(long amount)
        {
            try
            {

                var shippingPrices = _context.ShippingPrices.OrderBy(p => p.MaxCartOverallPrice).Where(p => p.MaxCartOverallPrice >= amount && p.IsDeleted == false).ToList();
                return shippingPrices[0];
            }
            catch
            {

            }

            return null;

        }


    }
}
