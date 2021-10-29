using Bgt.Ocean.Models;
using Bgt.Ocean.Models.CustomerLocation;
using Bgt.Ocean.Service.ModelViews.ActualJobHeader;
using Bgt.Ocean.Service.ModelViews.Adhoc;
using Bgt.Ocean.Service.ModelViews.RunControls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Service.Mapping.Mappers
{
    public static class AdhocMapper
    {
        public static IEnumerable<CustomerLocationInternalDepartmentView> ConvertToCustomerLocationInternalDept(this IEnumerable<CustomerLocationInternalDepartmentModel> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<CustomerLocationInternalDepartmentModel>, IEnumerable<CustomerLocationInternalDepartmentView>>(model);
        }

        public static IEnumerable<JobLegsViewResult> ConvertToJobLegViewResult(this IEnumerable<Models.RunControl.JobLegsView> model)
        {
            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<Models.RunControl.JobLegsView>, IEnumerable<JobLegsViewResult>>(model);
        }

        //CassetteModelView

        #region ## Cash-Add

        public static IEnumerable<TTarget> ConvertToAtmCashAdd<TTarget>(this IEnumerable<CassetteModelView> src, Guid screenHeadGuid )
            where TTarget : class
        {
            foreach (var item in src)
            {
                item.SumHeadGuid = screenHeadGuid;
            }

            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<CassetteModelView>, IEnumerable<TTarget>>(src);
        }

        public static IEnumerable<TblMasterActualJobActualCount> ConvertToTblMasterActualJobActualCount(this IEnumerable<CassetteModelView> model, Guid screanHeadGuid)
        {
            model = model.Select(s => { s.SumHeadGuid = screanHeadGuid; return s; });

            return ServiceMapperBootstrapper.MapperService.Map<IEnumerable<CassetteModelView>, IEnumerable<TblMasterActualJobActualCount>>(model);
        }

     

        #endregion
    }
}
