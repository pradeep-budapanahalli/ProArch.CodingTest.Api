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
    public class InvoiceController : ControllerBase
    {
        private readonly GenericRepository<Invoice> repository;

        public InvoiceController(GenericRepository<Invoice> repository)
        {
            this.repository = repository;
        }

        // GET: api/Invoice
        [HttpGet]
        public IEnumerable<Invoice> GetInvoices()
        {
            return repository.Get();
        }

        // GET: api/Invoice/5
        [HttpGet("{id}")]
        public IActionResult GetInvoice([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invoice = repository.GetByID(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }

        // PUT: api/Invoice/5
        [HttpPut("{id}")]
        public IActionResult PutInvoice([FromRoute] int id, [FromBody] Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != invoice.Id)
            {
                return BadRequest();
            }            

            try
            {
                repository.Update(invoice);
                repository.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
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

        // POST: api/Invoice
        [HttpPost]
        public IActionResult PostInvoice([FromBody] Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            repository.Insert(invoice);
            repository.Save();

            return CreatedAtAction("GetInvoice", new { id = invoice.Id }, invoice);
        }

        // DELETE: api/Invoice/5
        [HttpDelete("{id}")]
        public IActionResult DeleteInvoice([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invoice = repository.GetByID(id);
            if (invoice == null)
            {
                return NotFound();
            }

            repository.Delete(invoice);
            repository.Save();

            return Ok(invoice);
        }

        private bool InvoiceExists(int id)
        {
            return repository.Get().Any(e => e.Id == id);
        }
    }
}