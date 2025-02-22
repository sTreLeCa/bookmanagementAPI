namespace BookManagementAPI.Repositories
{
    using BookManagementAPI.Models;
    using MongoDB.Driver;
    using MongoDB.Bson;

    public class BookRepository
    {
        private readonly IMongoCollection<Books> _books;

        public BookRepository(IMongoDatabase database)
        {
            _books = database.GetCollection<Books>("books");
        }

        // Get a list of books sorted by views count with pagination
        public async Task<List<string>> GetBooksByPage(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            var books = await _books
                .Find(x => !x.IsDeleted)
                .SortByDescending(x => x.ViewsCount)
                .Skip(skip)
                .Limit(pageSize)
                .Project(x => x.Title)
                .ToListAsync();
            foreach(String bookss in books){
               var bax = await Getbytitle(bookss);
               if (bax != null){
               bax.ViewsCount++;
               await Update(bax.Id,bax);}
            }
            
            return books;
        }

        // Get book details by ID
        public async Task<Books?> Get(ObjectId id)
        {
            var book = await _books
                .Find(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (book != null)
            {
                book.ViewsCount++;  // Increment views count when the book is accessed
                await Update(id, book);
            }

            return book;
        }

        // Create a book
        public async Task Create(Books book) => await _books.InsertOneAsync(book);

        // Create many books
        public async Task CreateMany(List<Books> books) => await _books.InsertManyAsync(books);

        // Update a book
        public async Task Update(ObjectId id, Books book) => await _books.ReplaceOneAsync(x => x.Id == id, book);

        // Soft delete a book
        public async Task SoftDelete(ObjectId id) => await _books.UpdateOneAsync(x => x.Id == id, Builders<Books>.Update
            .Set(x => x.IsDeleted, true)
            .Set(x => x.DeletedDate, DateTime.UtcNow));

        // Soft delete many books
        public async Task SoftDeleteMany(List<ObjectId> ids)
    {
        // Update books where the Id is in the provided list of ObjectIds
        var filter = Builders<Books>.Filter.In(book => book.Id, ids);
        var update = Builders<Books>.Update
            .Set(book => book.IsDeleted, true)
            .Set(book => book.DeletedDate, DateTime.UtcNow);

        // Perform the update for all matching books
        await _books.UpdateManyAsync(filter, update);
    }


        // Get all book titles
        public async Task<List<string>> GetAllTitles() => await _books.Find(_ => true).Project(x => x.Title).ToListAsync();
         public async Task<Books?> Getbytitle(String title)
        {
          // Perform the asynchronous query to find the book by its title
         var book = await _books
         .Find(x => x.Title == title)
         .FirstOrDefaultAsync();  // Await the result of the async query
 
           return book;  // Return the book, or null if not found
        }

    }
}
