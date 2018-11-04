using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProArch.CodingTest.Common.Models;
using ProArch.CodingTest.Common.Services;

namespace ProArch.CodingTest.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/SpendSummary")]
    public class SpendSummaryController : Controller
    {
        private ISpendService spendService;

        public SpendSummaryController(ISpendService spendService)
        {
            this.spendService = spendService;
        }
        // GET: api/SpendSummay/5
        [HttpGet("{id}", Name = "Get")]
        public SpendSummary Get(int id)
        {
            return spendService.GetTotalSpend(id);
        }
    }
}
