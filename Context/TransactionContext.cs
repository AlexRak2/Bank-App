using ExpensesApp.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace ExpensesApp.Context
{
    public struct MonthlyData 
    {
        public string Month { get; set; }
        public decimal Amount { get; set; }

        public MonthlyData(string month, decimal amount)
        {
            Month = month;
            Amount = amount;
        }
    }
    public class TransactionContext
    {
        public string ConnectionString { get; set; }

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public TransactionContext(string connectionString) 
        {
            this.ConnectionString = connectionString;
        }


        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public void RefreshTransaction(string? sortOrder)
        {
            List<Transaction> list = new List<Transaction>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM transactions", conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Transaction()
                        {
                            Id = reader.GetInt32("ID"),
                            Amount = reader.GetInt32("amount"),
                            Transaction_Description = reader.GetString("transaction_description"),
                            Date = reader.GetDateTime("transaction_date"),
                            Transaction_Type = reader.GetString("transactionType")
                        });
                    }
                }
            }

            if (sortOrder != null)
            {
                if (sortOrder == "asc")
                    list = list.OrderBy(e => e.Amount).ToList();
                else
                    list = list.OrderByDescending(e => e.Amount).ToList();

            }
            else 
            {
                list = list.OrderByDescending(e => e.Date).ToList();
            }



            Transactions = list;
        }

        public Transaction GetTransactionById(int? Id) 
        {
            if (Id == null) return null;

            Transaction expenseInDb = Transactions.SingleOrDefault(expense => expense.Id == Id);
            return expenseInDb;
        }
        public void AddTransaction(Transaction transaction) 
        {
            using (MySqlConnection conn = GetConnection())
            {
                string date = (transaction.Date ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm");

                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"INSERT INTO transactions (Id, amount, transaction_description,transaction_date, transactionType) VALUES (0,'{transaction.Amount}', '{transaction.Transaction_Description}', '{date}', '{transaction.Transaction_Type}')", conn);
                cmd.ExecuteNonQuery();
            }
        }
        public void EditTransaction(Transaction transaction)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string date = (transaction.Date ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm");
                MySqlCommand cmd = new MySqlCommand($"UPDATE transactions SET amount = '{transaction.Amount}', transaction_description = '{transaction.Transaction_Description}', transaction_date='{date}', transactionType='{transaction.Transaction_Type}' WHERE Id = '{transaction.Id}'", conn);
                cmd.ExecuteNonQuery();
            }
        }
        public void RemoveTransaction(int? Id)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"DELETE FROM transactions WHERE Id = '{Id}';", conn);
                cmd.ExecuteNonQuery();
            }
        }


        public (List<MonthlyData>, List<MonthlyData> ExpenseTransactions) GetTransactionMonthlyData()
        {
            var incomeMonthlyTransactionS = GetMonthlyTotals(Transactions
                .Where(t => t.Transaction_Type == "INCOME")
                .ToList());

            var expenseMonthlyTransactions = GetMonthlyTotals(Transactions
                .Where(t => t.Transaction_Type == "EXPENSE")
                .ToList());

            return (incomeMonthlyTransactionS, expenseMonthlyTransactions);
        }

        public static List<MonthlyData> GetMonthlyTotals(List<Transaction> transactions)
        {
            List<MonthlyData> monthlyDatas = new List<MonthlyData>();

            var monthlyTotals = transactions
                .Where(t => t.Date.HasValue) // Filter out transactions with null Date
                .GroupBy(t => new { Year = t.Date.Value.Year, Month = t.Date.Value.Month })
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"), // Format as "Month Year"
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .OrderBy(x => DateTime.ParseExact(x.Month, "MMMM yyyy", null)); // Sort by month and year

            foreach (var transaction in monthlyTotals) 
            {
                monthlyDatas.Add(new MonthlyData(transaction.Month, transaction.TotalAmount));
            }

            return monthlyDatas;
        }
    }
}
