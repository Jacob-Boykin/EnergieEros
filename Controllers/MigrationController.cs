using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnergieEros.Controllers
{
    [Route("migration")]
    [ApiController]
    public class MigrationController : Controller
    {
        private readonly DataMigration _dataMigration;

        public MigrationController(DataMigration dataMigration)
        {
            _dataMigration = dataMigration;
        }

        [Route("run")]
        [HttpPost]
        public async Task<IActionResult> RunMigration()
        {
            await _dataMigration.MigrateUserData();
            return Ok("Migration completed successfully.");
        }
    }
}
