using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MRP_DAL.Helpers;
using MRP_DAL.Repository;
using MRP_DAL;
using ExternalModels.Dto;

namespace MRP_Admin_Api.Controllers
{
    public class ResultController : Controller
    {
        private readonly ResultHelper _helper;

        public ResultController(DbContextOptions<AppDbContext> db)
        {
            _helper = new ResultHelper(db);
        }

        public async Task<IActionResult> GetResult(DateTime dateTimeNow)
            => View(await _helper.GetNeededItems(dateTimeNow));
    }
}
