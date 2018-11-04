using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProArch.CodingTest.Common.Models;
using ProArch.CodingTest.Common.Repository;
using ProArch.CodingTest.Repository;

namespace ProArch.CodingTest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly GenericRepository<Supplier> repository;

        public SupplierController(GenericRepository<Supplier> repository)
        {
            this.repository = repository;
        }

        // GET: api/Supplier
        [HttpGet]
        public IEnumerable<Supplier> GetSuppliers()
        {
            return repository.Get();
        }

        // GET: api/Supplier/5
        [HttpGet("{id}")]
        public IActionResult GetSupplier([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplier = repository.GetByID(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return Ok(supplier);
        }

        // PUT: api/Supplier/5
        [HttpPut("{id}")]
        public IActionResult PutSupplier([FromRoute] int id, [FromBody] Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != supplier.Id)
            {
                return BadRequest();
            }

            try
            {
                repository.Update(supplier);
                repository.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Supplier
        [HttpPost]
        public IActionResult PostSupplier([FromBody] Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            repository.Insert(supplier);
            repository.Save();

            return CreatedAtAction("GetSupplier", new { id = supplier.Id }, supplier);
        }

        // DELETE: api/Supplier/5
        [HttpDelete("{id}")]
        public IActionResult DeleteSupplier([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplier = repository.GetByID(id);
            if (supplier == null)
            {
                return NotFound();
            }

            repository.Delete(supplier);
            repository.Save();

            return Ok(supplier);
        }

        private bool SupplierExists(int id)
        {
            return repository.Get().Any(e => e.Id == id);
        }
    }
}