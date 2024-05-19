using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Helpers;
using MRP_DAL.Repository;
using MRP_DAL;
using ExternalModels.Dto;

namespace MRP_Admin_Api.Controllers
{
    [ApiController]

    [Route("api/admin/result")]
    public class ResultController : Controller
    {
        private readonly ResultHelper _helper;

        public ResultController(DbContextOptions<AppDbContext> db)
        {
            _helper = new ResultHelper(db);
        }

        [HttpPut("get-result-tree")]
        public async Task<List<NeededItems>> MakeAnAccount(Guid orderId)
            => await _helper.GetNeededItems(orderId);
    }
}
