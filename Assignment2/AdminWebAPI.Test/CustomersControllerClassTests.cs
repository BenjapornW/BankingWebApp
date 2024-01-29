using System;
using System.ComponentModel.DataAnnotations;
using DataModelLibrary.Data;
using AdminWebAPI.Controllers;

namespace AdminWebAPI.Test
{
	public class CustomersControllerClassTests
	{
        private readonly McbaContext _context;

        public CustomersControllerClassTests()
		{
            _context = new McbaContext(new DbContextOptionsBuilder<McbaContext>().
            UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);
        }
	}
}

