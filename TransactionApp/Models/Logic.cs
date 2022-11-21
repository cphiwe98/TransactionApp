using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TransactionApp.ViewModels;

namespace TransactionApp.Models
{
    public class Logic
    {
        public static TransactionEntities db = new TransactionEntities();
        public static string connection = ConfigurationManager.ConnectionStrings["TransactionConn"].ConnectionString;
        /// <summary>
        /// returns a list of transaction
        /// </summary>
        /// <returns></returns>
        public static List<TransactionVM> GetTransactions()
        {
            using(var con =new SqlConnection(connection))
            {
                var transactions = con.Query<TransactionVM>("getTransactions", new { },commandType: CommandType.StoredProcedure).ToList();
                return transactions;
            }
            
        }
        /// <summary>
        /// Returns a list of clients
        /// </summary>
        /// <returns></returns>
        public static List<Client> GetClients()
        {
            using (var con = new SqlConnection(connection))
            {
                var client = con.Query<Client>("getClients", new { }, commandType: CommandType.StoredProcedure).ToList();
                return client;
            }

        }
        /// <summary>
        /// calculates client balance
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="typeId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Decimal CalcBalance(int clientId, int typeId, decimal amount)
        {
            Client client = Logic.GetClients().FirstOrDefault(x => x.ClientId == clientId);
            if (typeId == 1)
            {
                client.ClientBalance = (decimal)client.ClientBalance - (decimal)amount;
            }
            else
            {
                client.ClientBalance = (decimal)client.ClientBalance + (decimal)amount;
            }
            return (decimal)client.ClientBalance;
        }

        public static TransactionVM AssignValues(Transaction transaction)
        {
            TransactionVM transVm = new TransactionVM();

            transVm.TransactionId = transaction.TransactionId;
            transVm.Amount = transaction.Amount;
            transVm.ClientId = transaction.ClientId;
            transVm.TransactionTypeId = transaction.TransactionTypeId;
            transVm.TransactionTypeName = db.TransactionTypes.FirstOrDefault(x => x.TransactionTypeId == transaction.TransactionTypeId).TransactionTypeName;
            transVm.Name = db.Clients.FirstOrDefault(x => x.ClientId == transaction.ClientId).Name;
            transVm.Surname = db.Clients.FirstOrDefault(x => x.ClientId == transaction.ClientId).Surname;
            transVm.Comment = transaction.Comment;
            transVm.ClientBalance = db.Clients.FirstOrDefault(x => x.ClientId == transaction.ClientId).ClientBalance;

            return transVm;
        }

    }
}