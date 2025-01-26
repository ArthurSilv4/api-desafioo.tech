namespace api_desafioo.tech.Models
{
    public class Challenge
    {
        public Guid Id { get; init; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Dificulty { get; private set; }
        public string Category { get; private set; }
        public List<string>? Links { get; private set; }

        public User Author { get; private set; }
        public Guid AuthorId { get; private set; }
        public string AuthorName { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private Challenge() { }
        
        
        public Challenge(string title, string description, string dificulty, string category, User author, List<string>? links)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Dificulty = dificulty;
            Category = category;
            CreatedAt = DateTime.Now;
            Author = author;
            AuthorId = author.Id;
            AuthorName = author.Name;
            Links = links ?? new List<string>();
        }
    }
}
