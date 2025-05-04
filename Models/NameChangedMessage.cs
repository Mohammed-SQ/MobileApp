namespace FMMSRestaurant.Models;

public record NameChangedMessage(string Name)
{
    public static NameChangedMessage From(string name) => new(name);
}