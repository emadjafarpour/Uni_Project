
using GirlyMerely.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlyMerely.Infrastructure.Repositories
{
    public class LinkRepository : BaseRepository<Link, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;

        public LinkRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}
