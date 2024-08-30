using ExpensesApp.Context;
using ExpensesApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace ExpensesApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private TransactionContext? _context;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _context = HttpContext.RequestServices.GetService(typeof(TransactionContext)) as TransactionContext;
            _context.RefreshTransaction(HttpContext.Session.GetString("SortOrder"));

            return View();
        }

        public IActionResult Transactions() 
        {
            _context = HttpContext.RequestServices.GetService(typeof(TransactionContext)) as TransactionContext;
            _context.RefreshTransaction(HttpContext.Session.GetString("SortOrder"));

            return _context != null ? View(_context.Transactions) : View();
        }

        public IActionResult TransactionsSortToggle() 
        {
            string sortOrder = HttpContext.Session.GetString("SortOrder");

            if(string.IsNullOrEmpty(sortOrder))
                HttpContext.Session.SetString("SortOrder", "des");
            else
                HttpContext.Session.SetString("SortOrder", sortOrder == "asc" ? "des" : "asc");


            return RedirectToAction("Transactions");
        }

        public IActionResult CreateEditTransactions(int? Id)
        {
            _context = HttpContext.RequestServices.GetService(typeof(TransactionContext)) as TransactionContext;

            if (Id != null) 
            {
                var transactionInDb = _context.GetTransactionById(Id);
                return View(transactionInDb);
            }
            return View();
        }
        public IActionResult DeleteTransaction(int? Id)
        {
            _context = HttpContext.RequestServices.GetService(typeof(TransactionContext)) as TransactionContext;
            _context.RemoveTransaction(Id);

            return RedirectToAction("Transactions");
        }

        public IActionResult CreateEditTransactionsForm(Transaction model)
        {
            _context = HttpContext.RequestServices.GetService(typeof(TransactionContext)) as TransactionContext;

            if (_context.GetTransactionById(model.Id) != null)
                _context.EditTransaction(model);
            else
                _context.AddTransaction(model);

            return RedirectToAction("Transactions");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
