using Bgt.Ocean.Infrastructure.Util;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Service.ModelViews.Reports;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Linq;

namespace Bgt.Ocean.Service.Implementations
{
    public interface IExportExcelService
    {
        byte[] ExcelProductivityCollection(ReportProductivityCollection reportProductivity);
    }

    public class ExportExcelService : IExportExcelService
    {
        public readonly IMasterReportRepository _masterReportRepository;
        public ExportExcelService(
            IMasterReportRepository masterReportRepository)
        {
            _masterReportRepository = masterReportRepository;
        }

        public byte[] ExcelProductivityCollection(ReportProductivityCollection reportProductivity)
        {
            MemoryStream result = new MemoryStream();

            // Coding Write excel
            switch (reportProductivity.ReportStyleId)
            {
                case (int)EnumReport.ReportStyleId.ProductivityMex: // Mex
                    result = RenderProductivityExcel_Mex(reportProductivity.BrinkSiteGuid.Value, reportProductivity.BeginDate, reportProductivity.EndDate, reportProductivity.UserName);
                    break;
                default:
                    break;
            }

            return result.ToArray();
        }

        private MemoryStream RenderProductivityExcel_Mex(Guid brinksSiteGuid, string dateStart, string dateStop, string userName)
        {
            MemoryStream result = new MemoryStream();
            IWorkbook workbook = new XSSFWorkbook();   // for *.XLSX format
            ISheet sheet;
            ICell cell;

            var data = _masterReportRepository.Func_Report_Productivity_Mex_Get(dateStart, dateStop, brinksSiteGuid.ToString(), userName).ToList();
            sheet = workbook.CreateSheet("ProductivityReport");

            #region Set column width

            sheet.SetColumnWidth(0, (int)((10 + 0.72) * 256));// RouteGroup

            sheet.SetColumnWidth(1, (int)((10 + 0.72) * 256));// RouteDetail

            sheet.SetColumnWidth(2, (int)((10 + 0.72) * 256));// RunResourceShift

            sheet.SetColumnWidth(3, (int)((10 + 0.72) * 256)); // WorkDate

            sheet.SetColumnWidth(4, (int)((10 + 0.72) * 256));// DispatchTime

            sheet.SetColumnWidth(5, (int)((10 + 0.72) * 256));// ArrivalRunTime

            sheet.SetColumnWidth(6, (int)((10 + 0.72) * 256));// CloseRunTime

            sheet.SetColumnWidth(7, (int)((10 + 0.72) * 256));// ServiceJobNameAbb

            sheet.SetColumnWidth(8, (int)((15 + 0.72) * 256));// ServiceJobTypeName

            sheet.SetColumnWidth(9, (int)((10 + 0.72) * 256));// ScheduleTime

            sheet.SetColumnWidth(10, (int)((10 + 0.72) * 256));// ArrivalTime

            sheet.SetColumnWidth(11, (int)((10 + 0.72) * 256));// ActualTime

            sheet.SetColumnWidth(12, (int)((10 + 0.72) * 256));// DepartTime

            sheet.SetColumnWidth(13, (int)((20 + 0.72) * 256));// ServiceHour

            sheet.SetColumnWidth(14, (int)((15 + 0.72) * 256));// CustomerCode

            sheet.SetColumnWidth(15, (int)((30 + 0.72) * 256));// CustomerName

            sheet.SetColumnWidth(16, (int)((20 + 0.72) * 256));// CustomerLocationAddress

            sheet.SetColumnWidth(17, (int)((20 + 0.72) * 256));// Remarks

            sheet.SetColumnWidth(18, (int)((8 + 0.72) * 256));// FlagCancelAll

            sheet.SetColumnWidth(19, (int)((15 + 0.72) * 256));// ResonCancelText

            sheet.SetColumnWidth(20, (int)((20 + 0.72) * 256));// UnableToServiceCode

            sheet.SetColumnWidth(21, (int)((20 + 0.72) * 256));// ReasonText



            #endregion

            #region Set font style

            var font = workbook.CreateFont();
            font.FontHeightInPoints = 8;
            font.FontName = "Calibri";

            #endregion

            #region Set style header

            // Style the cell with borders all around.
            ICellStyle styleHerder = workbook.CreateCellStyle();
            styleHerder.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            styleHerder.BottomBorderColor = IndexedColors.Black.Index;
            styleHerder.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            styleHerder.LeftBorderColor = IndexedColors.Black.Index;
            styleHerder.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            styleHerder.RightBorderColor = IndexedColors.Black.Index;
            styleHerder.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            styleHerder.TopBorderColor = IndexedColors.Black.Index;

            styleHerder.FillForegroundColor = IndexedColors.Orange.Index;
            styleHerder.FillPattern = FillPattern.SolidForeground;
            styleHerder.SetFont(font);


            #endregion

            #region Sheet header
            var rows = sheet.CreateRow(2);

            cell = rows.CreateCell(0);
            cell.SetCellValue("RouteGroup");
            cell.CellStyle = styleHerder;


            cell = rows.CreateCell(1);
            cell.SetCellValue("RouteDetail");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(2);
            cell.SetCellValue("RunResourceShift");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(3);
            cell.SetCellValue("WorkDate");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(4);
            cell.SetCellValue("DispatchTime");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(5);
            cell.SetCellValue("ArrivalRunTime");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(6);
            cell.SetCellValue("CloseRunTime");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(7);
            cell.SetCellValue("ServiceJobNameAbb");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(8);
            cell.SetCellValue("ServiceJobTypeName");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(9);
            cell.SetCellValue("ScheduleTime");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(10);
            cell.SetCellValue("ArrivalTime");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(11);
            cell.SetCellValue("ActualTime");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(12);
            cell.SetCellValue("DepartTime");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(13);
            cell.SetCellValue("ServiceHour");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(14);
            cell.SetCellValue("CustomerCode");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(15);
            cell.SetCellValue("CustomerName");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(16);
            cell.SetCellValue("CustomerLocationAddress");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(17);
            cell.SetCellValue("Remarks");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(18);
            cell.SetCellValue("FlagCancelAll");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(19);
            cell.SetCellValue("ResonCancelText");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(20);
            cell.SetCellValue("UnableToServiceCode");
            cell.CellStyle = styleHerder;

            cell = rows.CreateCell(21);
            cell.SetCellValue("ReasonText");
            cell.CellStyle = styleHerder;


            #endregion

            #region Set style data

            // Style the cell with borders all around.
            ICellStyle style = workbook.CreateCellStyle();
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BottomBorderColor = IndexedColors.Black.Index;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.LeftBorderColor = IndexedColors.Black.Index;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.RightBorderColor = IndexedColors.Black.Index;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.TopBorderColor = IndexedColors.Black.Index;

            style.SetFont(font);
            #endregion

            int row = 3;
            #region Data
            foreach (var rw in data)
            {
                #region Data
                rows = sheet.CreateRow(row++);

                cell = rows.CreateCell(0);// RouteGroup
                cell.SetCellValue(rw.RouteGroup);
                cell.CellStyle = style;

                cell = rows.CreateCell(1);// RouteDetail
                cell.SetCellValue(rw.RouteDetail);
                cell.CellStyle = style;

                cell = rows.CreateCell(2);// RunResourceShift
                cell.SetCellValue(rw.RunResourceShift);
                cell.CellStyle = style;

                cell = rows.CreateCell(3);// WorkDate
                cell.SetCellValue(rw.WorkDate.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(4);// DispatchTime
                cell.SetCellValue(rw.DispatchTime.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(5);// ArrivalRunTime
                cell.SetCellValue(rw.ArrivalRunTime.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(6);// CloseRunTime
                cell.SetCellValue(rw.CloseRunTime.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(7);// ServiceJobNameAbb
                cell.SetCellValue(rw.ServiceJobNameAbb.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(8);// ServiceJobTypeName
                cell.SetCellValue(rw.ServiceJobTypeName.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(9);// ScheduleTime
                cell.SetCellValue(rw.ScheduleTime.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(10);// ArrivalTime
                cell.SetCellValue(rw.ArrivalTime.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(11);// ActualTime
                cell.SetCellValue(rw.ActualTime.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(12);// DepartTime
                cell.SetCellValue(rw.DepartTime.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(13);// ServiceHour
                cell.SetCellValue(rw.ServiceHour.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(14);// CustomerCode
                cell.SetCellValue(rw.CustomerCode.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(15);// CustomerName
                cell.SetCellValue(rw.CustomerName.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(16);// CustomerLocationAddress
                cell.SetCellValue(rw.CustomerLocationAddress.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(17);// Remarks
                cell.SetCellValue(rw.Remarks.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(18);// FlagCancelAll
                cell.SetCellValue(rw.FlagCancelAll.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(19);// ResonCancelText
                cell.SetCellValue(rw.ResonCancelText.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(20);// UnableToServiceCode
                cell.SetCellValue(rw.UnableToServiceCode.ToString());
                cell.CellStyle = style;

                cell = rows.CreateCell(21);// ReasonText
                cell.SetCellValue(rw.ReasonText.ToString());
                cell.CellStyle = style;
                #endregion
            }
            #endregion

            workbook.Write(result);
            return result;
        }
    }
}
