using Homework_March_17.Data;
using Homework_March_17.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace Homework_March_17.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Image; Integrated Security=true;";

        private IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(Image image, IFormFile imageFile)
        {
            var fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            imageFile.CopyTo(fs);

            image.FileName = fileName;

            var repo = new ImagesRepository(_connectionString);
            repo.Add(image);



            return View(image);
        }

        public HttpContext GetHttpContext()
        {
            return HttpContext;
        }

        [HttpPost]
        public IActionResult ViewImage(int id, string password )
        {
            ImagesRepository repository = new(_connectionString);
            ViewImageViewModel vm = new();
            vm.CorrectCredentials = repository.PasswordMatch(id, password);
            vm.image = repository.GetImageForId(id);
            if (!vm.CorrectCredentials)
            {
                TempData["warning-message"] = "Incorrect credentials!";
            }
            else
            {
                List<int> allowedIds = HttpContext.Session.Get<List<int>>("allowedids");
                if (allowedIds == null)
                {
                    allowedIds = new List<int>();
                }
                allowedIds.Add(id);
                HttpContext.Session.Set("allowedids", allowedIds);
            }
            return Redirect($"/home/viewimage?id={id}");
        }
        public IActionResult ViewImage(int id)

        {
            ImagesRepository repository = new(_connectionString);
            ViewImageViewModel vm = new();
            vm.image = repository.GetImageForId(id);

            if (TempData["warning-message"] != null)
            {
                vm.Message = (string)TempData["warning-message"];
            }

            if (!HasPermissionToView(id))
            {
                vm.CorrectCredentials = false;
            }
            else
            {
                vm.CorrectCredentials = true;
                repository.IncrementViewCount(id);

            }
            return View(vm);
        }
        private bool HasPermissionToView(int id)
        {
            var allowedIds = HttpContext.Session.Get<List<int>>("allowedids");
            HttpContext.Session.Get<List<int>>("allowedids");
            if (allowedIds == null)
            {
                return false;
            }

            return allowedIds.Contains(id);

        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}