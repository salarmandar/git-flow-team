using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models.BaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
namespace Bgt.Ocean.Service.Helpers
{

    public static class PaginationHelper
    {

        public static PagingResponseBase<T> ToPagination<T>(IEnumerable<T> dataSource, PagingBase pagingBase)
    where T : class
        {
            List<T> result = null;

            #region For support server filtering

            if (pagingBase.Filter != null)
            {
                var filters = pagingBase.Filter.Filters.Where(x => !string.IsNullOrEmpty(x.Value));
                if (filters.Any())
                {
                    var objFilters = filters.Select((e, i) =>
                    {

                        String condition = String.Empty;
                        object value = e.Value.ToLower().TrimStart().TrimEnd();
                        switch (e.Type)
                        {
                            case "date":
                                value = e.Value.ChangeFromJsonStringToClientDate();
                                condition = $" {e.Field} != null AND Convert.ToDateTime({e.Field}).Date == @{i} ";
                                break;
                            case "number":
                                value = Convert.ToDecimal(e.Value);
                                condition = $" {e.Field} != null AND Convert.ToDecimal({e.Field}) == @{i} ";
                                break;
                            default:
                                condition = $" {e.Field} != null AND {e.Field}.ToString().ToLower().TrimStart().TrimEnd().{e.Operator.ToPascalString()}(@{i}) ";
                                break;
                        }
                        return new { condition, value };
                    });

                    var strCondition = string.Join(pagingBase.Filter.Logic, objFilters.Select(o => o.condition));
                    var values = objFilters.Select(o => o.value).ToArray();
                    dataSource = dataSource.AsQueryable().Where(strCondition, values);
                }
            }
            #endregion

            #region For support server side sorting
            if (!string.IsNullOrEmpty(pagingBase.SortBy))
                dataSource = dataSource.AsQueryable().OrderBy($"{pagingBase.SortBy} {pagingBase.SortWith}").ToList();
            #endregion

            #region For support server side paging
            result = dataSource.Skip(pagingBase.Skip).Take(pagingBase.NumberPerPage).ToList();
            #endregion


            return new PagingResponseBase<T>() { Data = result, Total = dataSource.Count() };
        }

    }
}
