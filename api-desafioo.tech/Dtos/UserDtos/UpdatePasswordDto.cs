namespace api_desafioo.tech.Dtos.UserDtos
{
    public record UpdatePasswordDto(string name, string description, string email, string[] roles);
}
