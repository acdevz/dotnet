using Microsoft.AspNetCore.Mvc;
using MvcWithPlaywrightDemo.Models;

namespace MvcWithPlaywrightDemo.Controllers
{
    public class ItemsController : Controller
    {
        private static List<Item> _items = new List<Item>();
        private static int _nextId = 1;

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Item item)
        {
            if (!string.IsNullOrEmpty(item.Name) && item.Price > 0)
            {
                item.Id = _nextId++;
                _items.Add(item);
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Name and Price are required");
            return View(item);
        }

        [HttpGet]
        public IActionResult Index() => View(_items);

    }
}
