namespace BookManagementAPI.Controllers
{
    using BookManagementAPI.Models;
    using BookManagementAPI.Services;
    using Microsoft.AspNetCore.Mvc;
    using MongoDB.Bson;

    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly BookService _bookService;

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        // GET /api/books
        [HttpGet]
        public async Task<IActionResult> GetBooksByPage([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var books = await _bookService.GetBooksByPage(page, pageSize);
            return Ok(books);  // Return 200 OK with the list of books
        }

        // GET /api/books/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] ObjectId id)
        {
            var book = await _bookService.Get(id);
            if (book == null)
            {
                return NotFound();  // Return 404 if the book is not found
            }
            return Ok(book);  // Return 200 OK with the book details
        }

        // POST /api/books
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Books book)
        {
            try
            {
                await _bookService.Create(book);
                return CreatedAtAction(nameof(Get), new { id = book.Id }, book);  // Return 201 Created
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });  // Return 400 BadRequest if there's an issue
            }
        }
        [HttpPost("many")]
       public async Task<IActionResult> CreateMany([FromBody] List<Books> books)
{
    try
    {
        // Call the service method and get any warnings
        var warnings = await _bookService.CreateMany(books);

        if (warnings.Any())
        {
            // If there are warnings, return them in the response
            return BadRequest(new { message = "Some books were not added due to duplicates.", warnings = warnings });
        }

        return CreatedAtAction(nameof(Get), new { id = books[0].Id }, books);  // Return 201 Created if everything is fine
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });  // Return 400 BadRequest if there's an issue
    }
}


        // PUT /api/books/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] ObjectId id, [FromBody] Books book)
        {
            try
            { book.Id = id;
                await _bookService.Update(id, book);
                return NoContent();  // Return 204 No Content on successful update
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });  // Return 400 BadRequest if there's an issue
            }
        }

        // DELETE /api/books/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete([FromRoute] ObjectId id)
        {
            try
            {
                await _bookService.SoftDelete(id);
                return NoContent();  // Return 204 No Content on successful deletion
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });  // Return 400 BadRequest if there's an issue
            }
        }

        
        
[HttpDelete]
public async Task<IActionResult> SoftDeleteMany([FromBody] List<string> ids)
{
    // Convert List of string ids to List of ObjectId
    if (ids == null || ids.Count == 0)
    {
        return BadRequest(new { message = "No valid IDs provided." });
    }

    // Convert the list of strings to ObjectId
    var objectIds = ids.Select(id => new ObjectId(id)).ToList();

    // Perform the soft delete operation
    await _bookService.SoftDeleteMany(objectIds);
    return NoContent();  // Return 204 No Content if successful
}


    }
}
