using Luval.Common.Security;
using Luval.Data.Interfaces;
using Luval.Data.Sql;
using Luval.GoalTracker.Web;
using Luval.Media.Gallery.Entities;
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
            var factory = new SqlServerUnitOfWorkFactory(sqlConnectionString);
            services.TryAddSingleton<IUnitOfWorkFactory>(factory);
        }
    }
}
