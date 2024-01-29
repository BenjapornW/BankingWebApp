using System;
using System.ComponentModel.DataAnnotations;
using DataModelLibrary.Data;
using AdminWebAPI.Controllers;

namespace AdminWebAPI.Test
{
	public class CustomerManagerTests
	{
        private readonly McbaContext _context;

        public CustomerManagerTests()
		{
            _context = new McbaContext(new DbContextOptionsBuilder<McbaContext>().
           UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);
            _context.Database.EnsureCreated();
        }
    }
	
}

