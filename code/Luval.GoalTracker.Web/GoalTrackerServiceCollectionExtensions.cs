using Luval.DataStore;
using Luval.DataStore.Database.SqlServer;
using Luval.GoalTracker.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Web
{
    public static class GoalTrackerServiceCollectionExtensions
    {
        public static void AddGoalTracker(this IServiceCollection services, string sqlConnectionString)
        {
            services.ConfigureOptions(typeof(GoalTrackerConfigurationOptions));
            var factory = new SqlUnitOfWorkFactory(sqlConnectionString);
            services.TryAddSingleton<IUnitOfWorkFactory>(factory);
        }
    }
}
