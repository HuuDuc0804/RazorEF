using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Article{
    [Key]
    public int Id { get; set; }
    [StringLength(255)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Required]
    public DateTime Created { get; set; }

    [Column(TypeName = "ntext")]
    public string? Content { get; set; }
}