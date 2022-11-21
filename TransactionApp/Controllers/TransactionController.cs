using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TransactionApp.Models;
using TransactionApp.ViewModels;

namespace TransactionApp.Controllers
{
    public class TransactionController : Controller
    {
        public TransactionEntities db = new TransactionEntities();
        // GET: Transaction
        public ActionResult Index(int? ClientId, string searchString)
        {
            List<TransactionVM> list = new List<TransactionVM>();
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Name");
            
            if (ClientId != null)
            {
                var trans = Logic.GetTransactions().FirstOrDefault(x => x.ClientId == ClientId);
                trans.TransactionTypeName = db.TransactionTypes.FirstOrDefault(x => x.TransactionTypeId == trans.TransactionTypeId).TransactionTypeName;

                list.Add(trans);
            }
            else if (!string.IsNullOrEmpty(searchString))
            {
                int input = Regex.Matches(searchString.Trim(), @"[a-zA-Z]").Count;
                if (input > 0)
                {
                    var results = Logic.GetTransactions().FindAll(x => x.Name.Contains(searchString) || x.Surname.Contains(searchString));
                    foreach (var t in results)
                    {
                        t.TransactionTypeName = db.TransactionTypes.FirstOrDefault(x => x.TransactionTypeId == t.TransactionTypeId).TransactionTypeName;
                        list.Add(t);
                    }
                }
                else
                {
                    decimal amount = Convert.ToDecimal(searchString);
                    var results = Logic.GetTransactions().FindAll(x => x.Amount == amount).ToList();
                    foreach (var t in results)
                    {
                        t.TransactionTypeName = db.TransactionTypes.FirstOrDefault(x => x.TransactionTypeId == t.TransactionTypeId).TransactionTypeName;
                        list.Add(t);
                    } 
                    
                }
                
            }
            else
            {                
                var transtList = Logic.GetTransactions();

                foreach (var item in transtList)
                {
                    item.TransactionTypeName = db.TransactionTypes.FirstOrDefault(x => x.TransactionTypeId == item.TransactionTypeId).TransactionTypeName;
                    list.Add(item);                    
                }

            }

            return View(list.OrderBy(x=>x.Name));
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            TransactionVM transVm = Logic.AssignValues(transaction);

            return View(transVm);
        }

        public ActionResult Edit(TransactionVM transaction)
        {
            Transaction trans = new Transaction();

            trans.TransactionId = transaction.TransactionId;
            trans.Amount = transaction.Amount;
            trans.ClientId = transaction.ClientId;
            trans.TransactionTypeId = transaction.TransactionTypeId;
            trans.Comment = transaction.Comment;

            db.Entry(trans).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Client = new SelectList(db.Clients, "ClientId", "Name");
            ViewBag.Transationtype = new SelectList(db.TransactionTypes, "TransactionTypeId", "TransactionTypeName");
            return View();
        }
        public ActionResult Create(Transaction transaction)
        {
            Client client = Logic.GetClients().FirstOrDefault(x => x.ClientId == transaction.ClientId);
            
            client.ClientBalance = Logic.CalcBalance((int)transaction.ClientId, (int)transaction.TransactionTypeId, (decimal)transaction.Amount);
            db.Entry(client).State = EntityState.Modified;
            db.SaveChanges();

            db.Transactions.Add(transaction);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public JsonResult CalBalance(int clientId, int typeId, decimal amount)
        {
            return Json(Logic.CalcBalance(clientId, typeId, amount), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOldBalance(int clientId)
        {
            decimal client = (decimal)Logic.GetClients().FirstOrDefault(x => x.ClientId == clientId).ClientBalance;

            return Json(client, JsonRequestBehavior.AllowGet);
        }
    }
}