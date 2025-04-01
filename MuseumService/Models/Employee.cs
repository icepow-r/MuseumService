namespace MuseumService.Models;

// Employee.cs
public class Employee
{
    public int EmployeeId { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    
    public ICollection<Exhibit> Exhibits { get; set; }
}