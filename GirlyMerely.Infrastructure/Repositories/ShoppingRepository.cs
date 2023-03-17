using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GirlyMerely.Core.Models;
using GirlyMerely.Infrastructure;
using GirlyMerely.Infrastructure.Repositories;

namespace GirlyMerely.Infratructure.Repositories
{
    public class ShoppingRepository : BaseRepository<DiscountCode, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ShoppingRepository(MyDbContext context,LogsRepository logger):base(context,logger)
        {
            _context = context;
            _logger = logger;
        }

        public DiscountCode GetActiveDiscountCode(string discountCodeStr, int customerId)
        {
            DateTime today = DateTime.Now;
            var discountCode = _context.DiscountCodes.FirstOrDefault(dc => dc.DiscountCodeStr == discountCodeStr && dc.CustomerId == customerId
            && dc.IsActive == true && dc.ActivationStartDate <= today && dc.ActivationEndDate >= today);
            return discountCode;
        }
        public bool AddDiscountCode(DiscountCode discountCode)
        {
            try
            {
                _context.DiscountCodes.Add(discountCode);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public void DeactiveDiscountCode(string discountCodeStr)
        {
            var discountCode = _context.DiscountCodes.FirstOrDefault(dc => dc.DiscountCodeStr == discountCodeStr);
            discountCode.IsActive = false;
            _context.SaveChanges();
        }
        public void DeactiveDiscountCode(int discountId)
        {
            var discountCode = _context.DiscountCodes.FirstOrDefault(dc => dc.Id == discountId);
            discountCode.IsActive = false;
            _context.SaveChanges();
        }
        public void ActivateDiscountCode(string discountCodeStr)
        {
            var discountCode = _context.DiscountCodes.FirstOrDefault(dc => dc.DiscountCodeStr == discountCodeStr);
            discountCode.IsActive = true;
            _context.SaveChanges();
        }
        public void ActivateDiscountCode(int discountId)
        {
            var discountCode = _context.DiscountCodes.FirstOrDefault(dc => dc.Id == discountId);
            discountCode.IsActive = true;
            _context.SaveChanges();
        }
        public List<DiscountCode> GetDiscountCodes()
        {
            return _context.DiscountCodes.Include(d => d.Customer).Include(d => d.Customer.User).Where(d => d.IsDeleted == false).OrderByDescending(d => d.InsertDate).ToList();
        }
        public DiscountCode GetDiscountCodeByCodeName(string discountCodeStr)
        {
            return _context.DiscountCodes.FirstOrDefault(d => d.DiscountCodeStr == discountCodeStr);
        }
        
    }
}
