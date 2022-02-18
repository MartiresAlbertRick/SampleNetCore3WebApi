using AD.CAAPS.API.Models;
using AD.CAAPS.Common;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace AD.CAAPS.API.Controllers
{
    public sealed class CustomEnableQueryAttribute : EnableQueryAttribute
    {
        private readonly ODataQuerySettings settings;

        public CustomEnableQueryAttribute(IOptions<ODataQuerySettings> options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));
            settings = options.Value;
        }

        public override IQueryable ApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
        {
            if (queryable is null) throw new ArgumentNullException(nameof(queryable));
            if (queryOptions is null) throw new ArgumentNullException(nameof(queryOptions));

            if (queryOptions.Top != null)
            {
                if (Utils.IntFirstNotNull(queryOptions.Top.Value) > settings.PageSize)
                    queryOptions.ApplyTo(queryable, new ODataQuerySettings { PageSize = queryOptions.Top.Value });
                else
                    queryOptions.ApplyTo(queryable, new ODataQuerySettings { PageSize = settings.PageSize });
            }
            else
                queryOptions.ApplyTo(queryable, new ODataQuerySettings { PageSize = settings.PageSize });

            return base.ApplyQuery(queryable, queryOptions);
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext == null)
                throw new ArgumentNullException(nameof(actionExecutedContext));

            if (actionExecutedContext.Result is ObjectResult objectResult)
                if (Utils.IsSuccessStatusCode(objectResult.StatusCode))
                    base.OnActionExecuted(actionExecutedContext);
        }
    }
}