using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Team_Thorsday.Models;

// The error happens if ": BaseModel" is missing here
[Table("api_keys")]
public class ApiKey : BaseModel
{
    [PrimaryKey("id")] public int Id { get; set; }

    [Column("user_id")] public int UserId { get; set; }

    [Column("key")] public string Key { get; set; } = string.Empty;

    [Column("created_at")] public DateTime CreatedAt { get; set; }

    [Column("is_active")] public bool IsActive { get; set; } = true;
}