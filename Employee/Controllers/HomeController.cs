using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Employee.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace Employee.Controllers
{
    	public class HomeController : Controller
    	{
		private readonly ILogger<HomeController> _logger;
    		private IMongoCollection<EmployeeEntity> collection;

//    		public HomeController()
        	public HomeController(ILogger<HomeController> logger)
   		{
			var ipaddr = Environment.GetEnvironmentVariable("MONGO");
			if (String.IsNullOrEmpty(ipaddr)) {
				ipaddr = "127.0.0.1";
			}	
			//MongoClient client = new MongoClient("mongodb://127.0.0.1:27017");
			//MongoClient client = new MongoClient("mongodb://172.17.0.1:27017");
			string connectionString = "mongodb://" + ipaddr + ":27017";
			MongoClient client = new MongoClient(connectionString);
			IMongoDatabase db = client.GetDatabase("mydb");
			collection = db.GetCollection<EmployeeEntity>("Employee");
//			this.collection = db.GetCollection<EmployeeEntity>("Employee");
//			IMongoCollection<EmployeeEntity> collection = db.GetCollection<EmployeeEntity>("Employee");

            		_logger = logger;
    		}

		public IActionResult Index()
		{
//			var model = collection.Find
//				(FilterDefinition<EmployeeEntity>.Empty).ToList();
			var model = collection.Find(a=>true).ToList();
			return View(model);
		}

		[HttpGet]
		public IActionResult Insert()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Insert(EmployeeEntity emp)
		{
			Console.WriteLine(emp.FirstName);
			collection.InsertOne(emp);
			ViewBag.Message = "Employee added successfully!";
			return View();
		}

		[HttpGet]
		public IActionResult Update(string id)
		{
			ObjectId oId = new ObjectId(id);
			Console.WriteLine(id);
			Console.WriteLine(oId);
			EmployeeEntity emp = collection.Find(e => e.Id == oId).FirstOrDefault();

			return View(emp);
		}

		[HttpPost]
		public IActionResult Update(string id,EmployeeEntity emp)
		{
			emp.Id = new ObjectId(id);
			Console.WriteLine(emp.Id);
			var filter = Builders<EmployeeEntity>.Filter.Eq("Id", emp.Id);
//			var updateDef = Builders<EmployeeEntity>.Update.Set("FirstName", emp.FirstName);
//			updateDef = updateDef.Set("LastName", emp.LastName);
			var updateDef = Builders<EmployeeEntity>.Update.Set("FirstName", emp.FirstName)
								       .Set("LastName", emp.LastName);
			var result = collection.UpdateOne(filter, updateDef);

			if (result.IsAcknowledged)
			{
				ViewBag.Message = "Employee updated successfully!";
			}
			else
			{
				ViewBag.Message = "Error while updating Employee!";
			}
			return View(emp);
		}

		public IActionResult ConfirmDelete(string id)
		{
			ObjectId oId = new ObjectId(id);
			EmployeeEntity emp = collection.Find(e => e.Id == oId).FirstOrDefault();
			return View(emp);
		}

		[HttpPost]
		public IActionResult Delete(string id)
		{
			ObjectId oId = new ObjectId(id);
			var result = collection.DeleteOne<EmployeeEntity> (e => e.Id == oId);
			if (result.IsAcknowledged)
			{
				TempData["Message"] = "Employee deleted successfully!";
			}
			else
			{
				TempData["Message"] = "Error while deleting Employee!";
			}
				return RedirectToAction("Index");
		}


		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
