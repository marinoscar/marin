using Luval.DataStore;
using Luval.GoalTracker.Entities;
using Luval.Web.Common.Filters;
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

    [Area("GoalTracker"), Authorize, ViewDataFilter("Manifest", "/lib/luval.goaltracker/manifest.json")]
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
        [HttpGet, Route("GoalTracker/Index")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {

            var items = (await GoalTrackerRepository.GetGoalViewAsync(await GetUserIdAsync(), cancellationToken)).OrderBy(i => i.Sort);
            return View(items);
        }

        [HttpGet, Route("GoalTracker/Overview/{id}")]
        public async Task<IActionResult> Overview(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id)) return View("Index");
            var goal = await GoalTrackerRepository.GetGoalAsync(id, cancellationToken);
            return View(goal);
        }

        [HttpGet, Route("GoalTracker/Add/{id}")]
        public async Task<IActionResult> Add(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id)) return View("Index");
            var goal = await GoalTrackerRepository.GetGoalAsync(id, cancellationToken);
            return View(new GoalEntryModelView() { DefinitionId = goal.Id, Question = goal.Question, Type = goal.Type, UnitOfMeasure = goal.UnitOfMeasure });
        }

        [HttpPost, Route("GoalTracker/Add")]
        public async Task<IActionResult> Add(GoalEntryModelView entry, CancellationToken cancellationToken)
        {
            var goalEntry = new GoalEntry()
            {
                GoalDefinitionId = entry.DefinitionId,
                GoalDateTime = DateTime.UtcNow.AddHours(-6).Date,
                NumericValue = entry.NumberValue
            };
            await GoalTrackerRepository.CreateOrUpdateEntryAsync(goalEntry, await GetUserIdAsync(), cancellationToken);
            return RedirectToAction("Index");
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

        [HttpGet, Route("GoalTracker/MultipleEntry")]
        public async Task<IActionResult> MultipleEntry(string frequency, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(frequency)) frequency = nameof(GoalFrequency.Daily);
            var model = new GoalPackageModelView() { DateTime = DateTime.UtcNow.AddHours(-6).Date };
            var items = (await GoalTrackerRepository.GetGoalsByFrequencyAsync(frequency, await GetUserIdAsync(), cancellationToken)).OrderBy(i => i.Sort);
            model.Questions.AddRange(items.Select(i => new GoalEntryModelView()
            {
                DefinitionId = i.Id,
                Question = i.Question,
                Type = i.Type,
                UnitOfMeasure = i.UnitOfMeasure
            }));
            return View(model);
        }

        [HttpPost, Route("GoalTracker/MultipleEntry")]
        public async Task<IActionResult> MultipleEntry(GoalPackageModelView payload, CancellationToken cancellationToken)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            var records = payload.Questions.Select(i => new GoalEntry() { GoalDateTime = payload.DateTime, NumericValue = i.NumberValue, GoalDefinitionId = i.DefinitionId });
            await GoalTrackerRepository.CreateOrUpdateEntryAsync(records, await GetUserIdAsync(), cancellationToken);
            return RedirectToAction("Index");
        }

        [HttpPost, Route("GoalTracker/UpdateProgress")]
        public async Task<IActionResult> UpdateProgress(CancellationToken cancellationToken)
        {
            var userId = await GetUserIdAsync();
            var goals = await GoalTrackerRepository.GetGoalsByUserIdAsync(await GetUserIdAsync(), cancellationToken);
            foreach (var goal in goals)
            {
                await GoalTrackerRepository.UpdateProgressAsync(goal, userId, cancellationToken);
            }
            return Ok();
        }

        private async Task<string> GetUserIdAsync()
        {
            var user = await UserRepository.GetUserAsync(User);
            if (user == null) return null;
            return user.Id;
        }
    }
}
