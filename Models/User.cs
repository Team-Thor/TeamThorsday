using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Team_Thorsday.Models;

// Check this one too, it also needs ": BaseModel"
[Table("users")]
public class User : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    // This property is not mapped to the DB, so no attribute needed
    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";
}