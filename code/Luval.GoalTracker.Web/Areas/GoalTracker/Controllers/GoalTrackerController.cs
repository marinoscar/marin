using Luval.Data.Interfaces;
using Luval.GoalTracker.Entities;
using Luval.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Web.Areas.GoalTracker.Controllers
{

    [Area("GoalTracker"), Authorize]
    public class GoalTrackerController : Controller
    {
        protected GoalTrackerRepository GoalTrackerRepository { get; private set; }
        protected ILogger<GoalTrackerController> Logger { get; private set; }
        protected IApplicationUserRepository UserRepository { get; set; }

        public GoalTrackerController(IUnitOfWorkFactory unitOfWorkFactory, ILogger<GoalTrackerController> logger, IApplicationUserRepository userRepository)
        {
            GoalTrackerRepository = new GoalTrackerRepository(unitOfWorkFactory);
            Logger = logger;
            UserRepository = userRepository;

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, Route("GoalTracker/Create")]
        public async Task<IActionResult> Create(GoalDefinition goal, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(Request.Body);
            var jsonPayload = await reader.ReadToEndAsync();
            Logger.LogInformation(jsonPayload);
            if (goal == null && goal.IsValid()) return BadRequest("Invalid payload");
            var user = await UserRepository.GetUserAsync(User);
            try
            {
                await GoalTrackerRepository.CreateOrUpdateGoalAsync(goal, user.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                throw;
            }
            return Ok();
        }

        [HttpPost, Route("GoalTracker/CreateBatch")]
        public async Task<IActionResult> CreateBatch(CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(Request.Body);
            var jsonPayload = await reader.ReadToEndAsync();
            Logger.LogInformation(jsonPayload);
            //if (goals == null || !goals.Any() || goals.Any(i => !i.IsValid())) return BadRequest("Invalid payload");
            //var user = await UserRepository.GetUserAsync(User);
            //foreach (var goal in goals)
            //{
            //    await GoalTrackerRepository.CreateOrUpdateGoalAsync(goal, user.Id, cancellationToken);
            //}
            return Ok();
        }

        [HttpGet, Route("GoalTracker/Entry")]
        public IActionResult Entry(string frequency)
        {
            var model = new GoalPackageModelView() { };
            model.Questions.Add(new GoalEntryModelView() { DefinitionId = "1", Question = "How many miles did you ran", Type = "", UnitOfMeasure = "units" });
            model.Questions.Add(new GoalEntryModelView() { DefinitionId = "2", Question = "How pushups you did", Type = "", UnitOfMeasure = "units" });
            model.Questions.Add(new GoalEntryModelView() { DefinitionId = "3", Question = "How many miles you walked", Type = "", UnitOfMeasure = "units" });
            model.Questions.Add(new GoalEntryModelView() { DefinitionId = "4", Question = "Did you go to the gym", Type = "Binary", UnitOfMeasure = "units" });
            model.Questions.Add(new GoalEntryModelView() { DefinitionId = "5", Question = "Did you got your vitamins", Type = "Binary", UnitOfMeasure = "units" });
            return View(model);
        }

        [HttpPost, Route("GoalTracker/Entry")]
        public async Task<IActionResult> Entry(GoalPackageModelView payload, CancellationToken cancellationToken)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            var records = payload.Questions.Select(i => new GoalEntry() { NumericValue = i.NumberValue, GoalDefinitionId = i.DefinitionId });
            var user = await UserRepository.GetUserAsync(User);
            await GoalTrackerRepository.CreateOrUpdateEntryAsync(records, user.Id, cancellationToken);
            return View();
        }
    }
}
