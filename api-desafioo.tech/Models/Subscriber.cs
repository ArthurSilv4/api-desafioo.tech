namespace api_desafioo.tech.Models
{
    public class Subscriber
    {
        public Guid Id { get; private init; }
        public string Email { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Subscriber(string email)
        {
            Id = Guid.NewGuid();
            Email = email;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
