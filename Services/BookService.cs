namespace BookManagementAPI.Services
{
    using System.Runtime.CompilerServices;
    using BookManagementAPI.Models;
    using BookManagementAPI.Repositories;
    using MongoDB.Bson;

    public class BookService
    {
        private readonly BookRepository _bookRepository;

        public BookService(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // Get books with pagination
        public async Task<List<string>> GetBooksByPage(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            return await _bookRepository.GetBooksByPage(page, pageSize);
        }

        // Get a specific book
        public async Task<Books?> Get(ObjectId id)
        {
            var book = await _bookRepository.Get(id);

            // Calculate the popularity score on the fly
            if (book != null)
            {
                book.PopularityScore = book.ViewsCount * 0.5 + (DateTime.UtcNow.Year - book.PublicationYear) * 2;
            }

            return book;
        }

        // Create a book
        public async Task Create(Books book)
        { var book1 = await _bookRepository.Getbytitle(book.Title);
            if (book1 != null && book1.IsDeleted == false)
            {
                throw new Exception($"Book with this title {book.Title} already exists");
            }else if(book1 != null && book1.IsDeleted == true){ book.Id = book1.Id;  await _bookRepository.Update(book1.Id,book);
            }else await _bookRepository.Create(book); 
        }

        // Create many books
        public async Task<List<string>> CreateMany(List<Books> books)
{
    var warnings = new List<string>(); // List to store warnings or messages

    foreach (var book in books)
    {
        var book1 = await _bookRepository.Getbytitle(book.Title);

        if (book1 != null && book1.IsDeleted == false)
        {
            warnings.Add($"Book with this title {book.Title} already exists.");
        }
        else if (book1 != null && book1.IsDeleted == true)
        {
            book.Id = book1.Id;  
            await _bookRepository.Update(book1.Id, book);  // Update the existing deleted book
        }
        else
        {
            await _bookRepository.Create(book);  // Add the new book
        }
    }

    return warnings; // Return any warnings encountered during the process
}


        // Update a book
        public async Task Update(ObjectId id, Books book) => await _bookRepository.Update(id, book);

        // Soft delete a book
        public async Task SoftDelete(ObjectId id) => await _bookRepository.SoftDelete(id);

        // Soft delete many books
        public async Task SoftDeleteMany(List<ObjectId> ids) => await _bookRepository.SoftDeleteMany(ids);
    }
}
