using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManager.Data;
using InventoryManager.Models;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace StockManager.Controllers
{
    public class StockProductsController : Controller
    {
        private readonly InventoryManagerContext _context;

        public StockProductsController(InventoryManagerContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {

            var products = await _context.StockProduct
    .Include(p => p.StockSublocations)
    .ToListAsync();

            // Call UpdateQuantity for each product
            foreach (var product in products)
            {
                // Ensure you handle null or empty lists if necessary
                product.Quantity = product.StockSublocations.Sum(s => s.SublocationQuantity);
            }

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.StockProduct
            .Include(sp => sp.StockSublocations)  // Ensure this is included
            .FirstOrDefault(sp => sp.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            else
            {
                product.Quantity = product.StockSublocations.Sum(s => s.SublocationQuantity);
            }
            

            return View(product);
        }
        public async Task<IActionResult> SearchProducts(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(await _context.StockProduct.ToListAsync());
            }

            var products = await _context.StockProduct
                .Where(p => p.SKU.Contains(query) || p.Name.Contains(query))
                .ToListAsync();

            return View("Index", products);
        }
    
    public bool SublocationExists(StockProduct prod, string sublocationName)
        {
            // Check if any sublocation in the product's sublocations list matches the provided name
            return prod.StockSublocations.Any(s => s.Sublocation == sublocationName);
        }

        public IActionResult AddStock()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AddStock(string sku, string newProductSublocation, int newQuantity)
        {
            // Fetch product and include sublocations
            var prod = await _context.StockProduct
                .Include(p => p.StockSublocations)
                .FirstOrDefaultAsync(p => p.SKU == sku);

            if (prod == null)
            {
                ModelState.AddModelError("sku", "The SKU does not exist. Create new product first, and then add to stock.");
                return View();
            }

            if (!SublocationExists(prod, newProductSublocation))
            {
                var newSublocation = new StockSublocation
                {
                    Sublocation = newProductSublocation,
                    SublocationQuantity = newQuantity,
                    ProductId = prod.Id // Ensure the foreign key is set

                };
                prod.StockSublocations.Add(newSublocation);

            }
            else
            {
                var existingSublocation = prod.StockSublocations
                    .FirstOrDefault(s => s.Sublocation == newProductSublocation);

                if (existingSublocation != null)
                {
                    // Update the quantity of the existing sublocation
                    existingSublocation.SublocationQuantity += newQuantity;
                }
            }
            prod.UpdateQuantity();

            await _context.SaveChangesAsync();



            return View(prod);
        }


        public IActionResult SearchSkus()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchSkus(string sku)
        {
            if (string.IsNullOrEmpty(sku))
            {
                ModelState.AddModelError("sku", "Please enter a SKU.");
                return View();
            }

            var product = await _context.StockProduct
                .Include(p => p.StockSublocations)
                .FirstOrDefaultAsync(p => p.SKU == sku);

            if (product == null)
            {
                ModelState.AddModelError("sku", "No product found with this SKU.");
                return View();
            }

            return View("RemoveStock", product);
        }
        public IActionResult RemoveStock()
        {
            return View();
        }
        // POST: StockProducts/RemoveStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStock(string sku, string productSublocation, int quantity)
        {
            var product = await _context.StockProduct
                .Include(p => p.StockSublocations)
                .FirstOrDefaultAsync(p => p.SKU == sku);

            if (product == null)
            {
                ModelState.AddModelError("sku", "The SKU does not exist.");
                return RedirectToAction(nameof(SearchSkus));
            }

            var sublocation = product.StockSublocations.FirstOrDefault(s => s.Sublocation == productSublocation);

            if (sublocation == null)
            {
                ModelState.AddModelError("selectedSublocation", "Sublocation not found.");
                return View("RemoveStock", product);
            }

            if (sublocation.SublocationQuantity < quantity)
            {
                ModelState.AddModelError("quantity", "Insufficient stock quantity.");
                return View("RemoveStock", product);
            }


            if (sublocation.SublocationQuantity > 0)
            {
                // Update the quantity of the existing sublocation
                sublocation.SublocationQuantity -= quantity;
                product.UpdateQuantity();
            }
        
            await _context.SaveChangesAsync();



            return View(product);
    }
    
    /*
    public IActionResult RemoveStock()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> RemoveStock(string sku, string productSublocation, int quantity)
            {
                // Fetch product and include sublocations
                var prod = await _context.StockProduct
                    .Include(p => p.StockSublocations)
                    .FirstOrDefaultAsync(p => p.SKU == sku);

                if (prod == null)
                {
                ModelState.AddModelError("sku", "The SKU does not exist. Create new product first, and then add to stock.");
                    return View();
                }

                if (!SublocationExists(prod, productSublocation))
                {
                ModelState.AddModelError("productSublocation", "Product not found in sublocation.");

                return View();

                }
                else
                {
                    var existingSublocation = prod.StockSublocations
                        .FirstOrDefault(s => s.Sublocation == productSublocation);

                    if (existingSublocation != null && existingSublocation.SublocationQuantity > 0)
                    {
                        // Update the quantity of the existing sublocation
                        existingSublocation.SublocationQuantity -= quantity;
                    }
                }

                // Calculate total quantity
                prod.UpdateQuantity();

            // Logging the updated quantities


            // Update and save changes
            await _context.SaveChangesAsync();



            return View(prod);
        }
    */

    public IActionResult StockChange()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> StockChange(string sku, string newProductSublocation, int newQuantity)
        {
            // Fetch product and include sublocations
            var prod = await _context.StockProduct
                .Include(p => p.StockSublocations)
                .FirstOrDefaultAsync(p => p.SKU == sku);

            if (prod == null)
            {
                return NotFound();
            }

            if (!SublocationExists(prod, newProductSublocation))
            {
                var newSublocation = new StockSublocation
                {
                    Sublocation = newProductSublocation,
                    SublocationQuantity = newQuantity,
                    ProductId = prod.Id // Ensure the foreign key is set

                };
                prod.StockSublocations.Add(newSublocation);
                
            }
            else
            {
                var existingSublocation = prod.StockSublocations
                    .FirstOrDefault(s => s.Sublocation == newProductSublocation);

                if (existingSublocation != null)
                {
                    // Update the quantity of the existing sublocation
                    existingSublocation.SublocationQuantity += newQuantity;
                }
            }

            // Calculate total quantity
            prod.UpdateQuantity();

            // Logging the updated quantities


            // Update and save changes
            _context.StockProduct.Update(prod);
            await _context.SaveChangesAsync();



            return View(prod);
        }







        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SKU,Name,Quantity,Price,Sublocations")] StockProduct product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.StockProduct.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }


        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SKU,Name,Quantity,Price,Sublocations")] StockProduct product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.StockProduct
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.StockProduct.FindAsync(id);
            if (product != null)
            {
                _context.StockProduct.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.StockProduct.Any(e => e.Id == id);
        }
    }
}
