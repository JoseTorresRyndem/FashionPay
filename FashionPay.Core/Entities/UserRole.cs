namespace FashionPay.Core.Entities;

public partial class UserRole
{
    public int IdUser { get; set; }

    public int IdRole { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}