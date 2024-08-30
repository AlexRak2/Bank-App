using ExpensesApp.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpensesApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class DataController : Controller
    {
        [HttpGet("GetChartData")]
        public ActionResult GetChartData()
        {
            TransactionContext _context = HttpContext.RequestServices.GetService(typeof(TransactionContext)) as TransactionContext;

            var (incomeTransactions, expenseTransactions) = _context.GetTransactionMonthlyData();
            ChartData chartData = new ChartData(incomeTransactions, expenseTransactions);

            return Json(chartData);
        }
    }

    public struct ChartData 
    {
        public List<MonthlyData> IncomeData { get; set; }
        public List<MonthlyData> ExpenseData { get; set; }

        public ChartData(List<MonthlyData> incomeData, List<MonthlyData> expenseData)
        {
            IncomeData = incomeData;
            ExpenseData = expenseData;
        }
    }
}
