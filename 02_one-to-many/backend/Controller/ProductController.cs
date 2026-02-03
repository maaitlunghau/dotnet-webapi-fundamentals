using backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public ProductController(DataContext dbContext) => _dbContext = dbContext;
    }
}
