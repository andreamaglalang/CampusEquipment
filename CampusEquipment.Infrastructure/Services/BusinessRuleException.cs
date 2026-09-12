namespace CampusEquipment.Infrastructure.Services;

public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}