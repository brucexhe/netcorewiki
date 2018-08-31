using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyWiki.Models;

namespace MyWiki.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Edit(string category, string title)
        {
            Document doc = GetDocument(category, title);

            ViewBag.CategoryList = GetCategorys();

            return View(doc);
        }

        public string GetUserName()
        {
            var username = System.Net.Dns.GetHostEntry(Request.HttpContext.Connection.RemoteIpAddress).HostName;
            username = username.Split(".phbdomain")[0].Split("-").Last();
            return username;
        }

        [HttpPost]
        public IActionResult Edit()
        {
            try
            {
                var newTitle = Request.Form["title"];
                var oldTitle = Request.Form["oldtitle"];
                var category = Request.Form["category"];
                var content = Request.Form["editorValue"];

                if (string.IsNullOrWhiteSpace(oldTitle))
                {
                    oldTitle = newTitle;
                }

                var oldpath = AppDomain.CurrentDomain.BaseDirectory + "doc\\" + category + "\\" + oldTitle.ToString().Trim()+ ".txt";
                var newpath = AppDomain.CurrentDomain.BaseDirectory + "doc\\" + category + "\\" + newTitle.ToString().Trim() + ".txt";
                var oldlogpath = AppDomain.CurrentDomain.BaseDirectory + "doc\\" + category + "\\" + oldTitle.ToString().Trim() + ".log";
                var newlogpath = AppDomain.CurrentDomain.BaseDirectory + "doc\\" + category + "\\" + newTitle.ToString().Trim() + ".log";

                if (newTitle != oldTitle)
                {
                    System.IO.File.Move(oldpath, newpath);
                    System.IO.File.Move(oldlogpath, newlogpath);
                }

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(newpath, false))
                {
                    sw.Write(content);
                }
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(newlogpath, true))
                {
                    sw.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ", " + GetUserName() + Environment.NewLine);
                }
                category = System.Net.WebUtility.UrlEncode(category);
                newTitle = System.Net.WebUtility.UrlEncode(newTitle);
                return Redirect("/?category=" + category + "#" + category + "/" + newTitle);
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public IActionResult Get(string category, string title)
        {
            Document doc = GetDocument(category, title);

            return Json(doc);
        }

        private Document GetDocument(string category, string title)
        {
            Document doc = new Document();
            doc.Category = category;
            doc.Title = title;
            if (string.IsNullOrWhiteSpace(title))
            {
                return doc;
            }
            try
            {
                //title = System.Net.WebUtility.UrlDecode(title);
                var path = AppDomain.CurrentDomain.BaseDirectory + "doc\\" + category + "\\" + title + ".txt";
                var logpath = AppDomain.CurrentDomain.BaseDirectory + "doc\\" + category + "\\" + title + ".log";
                using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
                {
                    doc.Content = sr.ReadToEnd();
                }
                doc.ChangeLogs = GetChangeLogs(logpath);

            }
            catch (Exception ex)
            {
                doc.Content = "Server Error:" + ex.Message;
            }

            return doc;
        }

        public IActionResult GetList(string category)
        {
            List<Document> list = new List<Document>();

            if (string.IsNullOrWhiteSpace(category))
            {
                var PATH = AppDomain.CurrentDomain.BaseDirectory + "doc\\";
                var dicts = System.IO.Directory.GetDirectories(PATH);
                foreach (var item in dicts)
                {
                    category = item.Substring(item.LastIndexOf('\\') + 1);
                    list.AddRange(GetDocuments(category, false));
                }
            }
            else
            {
                list.AddRange(GetDocuments(category, false));
            }


            return Json(list);

        }

        private List<Document> GetDocuments(string category, bool showContent)
        {
            List<Document> list = new List<Document>();
            var path = AppDomain.CurrentDomain.BaseDirectory + "doc\\" + category;
            var files = System.IO.Directory.GetFiles(path);
            foreach (var item in files)
            {
                if (item.EndsWith(".txt"))
                {
                    var i = item.LastIndexOf('\\');
                    Document doc = new Document();
                    doc.Category = category;
                    doc.Path = item;
                    doc.Title = item.Substring(i + 1, item.Length - i - 5);

                    list.Add(doc);
                }

            }

            return list;
        }

        private List<string> GetChangeLogs(string path)
        {
            List<string> result = new List<string>();

            try
            {

                using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
                {
                    var content = sr.ReadToEnd();
                    result = content.Split("\n").ToList();
                }

            }
            catch
            {
            }

            return result;
        }


        public IActionResult Index()
        {

            ViewBag.CategoryList = GetCategorys();
            return View();
        }
        private List<string> GetCategorys()
        {
            List<string> Categorys = new List<string>();

            var PATH = AppDomain.CurrentDomain.BaseDirectory + "doc\\";
            var dicts = System.IO.Directory.GetDirectories(PATH);
            foreach (var item in dicts)
            {
                Categorys.Add(item.Substring(item.LastIndexOf('\\') + 1));
            }
            return Categorys;
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
