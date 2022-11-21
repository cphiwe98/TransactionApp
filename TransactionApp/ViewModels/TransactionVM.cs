using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionApp.ViewModels
{
    public class TransactionVM
    {
        public long TransactionId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<short> TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; }
        public Nullable<int> ClientId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Nullable<decimal> ClientBalance { get; set; }
        public string Comment { get; set; }
    }
}