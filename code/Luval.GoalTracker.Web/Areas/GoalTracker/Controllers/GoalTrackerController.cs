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
using System.Reflection.Metadata;
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
        public async Task<IActionResult> CreateBatch(GoalBatch batch, CancellationToken cancellationToken)
        {
            var user = await UserRepository.GetUserAsync(User);
            try
            {
                foreach (var goal in batch.Goals)
                {
                    await GoalTrackerRepository.CreateOrUpdateGoalAsync(goal, user.Id, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                throw;
            }
            return Ok();
        }


        [HttpGet, Route("GoalTracker/Entry")]
        public async Task<IActionResult> Entry(string frequency, CancellationToken cancellationToken)
        {
            
            if (string.IsNullOrWhiteSpace(frequency)) frequency = nameof(GoalFrequency.Daily);
            var model = new GoalPackageModelView() { };
            var items = await GoalTrackerRepository.GetGoalsByFrequencyAsync(frequency, await GetUserIdAsync(), cancellationToken);
            model.Questions.AddRange( items.Select(i => new GoalEntryModelView() { 
                DefinitionId = i.Id, Question = i.Question, Type = i.Type, UnitOfMeasure = i.UnitOfMeasure
            }) );
            return View(model);
        }

        [HttpPost, Route("GoalTracker/Entry")]
        public async Task<IActionResult> Entry(GoalPackageModelView payload, CancellationToken cancellationToken)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            var records = payload.Questions.Select(i => new GoalEntry() { GoalDateTime = payload.DateTime,  NumericValue = i.NumberValue, GoalDefinitionId = i.DefinitionId });
            await GoalTrackerRepository.CreateOrUpdateEntryAsync(records, await GetUserIdAsync(), cancellationToken);
            return RedirectToAction("Index");
        }

        private async Task<string> GetUserIdAsync()
        {
            var user = await UserRepository.GetUserAsync(User);
            if (user == null) return null;
            return user.Id;
        }
    }
}
