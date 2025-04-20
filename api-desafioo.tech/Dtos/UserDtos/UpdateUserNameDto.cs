namespace api_desafioo.tech.Dtos.UserDtos
{
    public record UpdateUserNameDto(string name, string description, string email, string[] roles);
}
