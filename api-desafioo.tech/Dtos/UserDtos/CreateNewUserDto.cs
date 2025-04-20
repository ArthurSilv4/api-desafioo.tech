namespace api_desafioo.tech.Dtos.UserDtos
{
    public record CreateNewUserDto(string name, string description, string email, string[] roles);
}
