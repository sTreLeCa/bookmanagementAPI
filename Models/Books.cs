namespace BookManagementAPI.Models
{
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Books
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("title")]
        public required string Title { get; set; }

        [BsonElement("author name")]
        public required string AuthorName { get; set; }

        [BsonElement("publication year")]
        public required int PublicationYear { get; set; }

        [BsonElement("views count")]
        public  int ViewsCount { get; set; }

        [BsonElement("isDeleted")]
        public bool IsDeleted { get; set; } = false;

        [BsonElement("DeletedDate")]
        public DateTime? DeletedDate { get; set; } = null;

        [BsonIgnore]
        public double PopularityScore { get; set; }

        public static explicit operator Books(Task<Books?> v)
        {
            throw new NotImplementedException();
        }
    }
}
