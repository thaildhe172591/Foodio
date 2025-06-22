namespace FoodioAPI.DTOs.User;

public class QueryUserDTO : QueryParameters
{
    public QueryUserDTO()
    {
        OrderBy = "UserName";
    }

}
