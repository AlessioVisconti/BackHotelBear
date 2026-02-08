using System.ComponentModel.DataAnnotations;

public class CreateRoomDto
{
    [Required, MaxLength(5)]
    public string RoomNumber { get; set; } = null!;

    [Required, MaxLength(30)]
    public string RoomName { get; set; } = null!;

    [MaxLength(150)]
    public string? Description { get; set; }

    [Required]
    public int Beds { get; set; }

    [Required, MaxLength(20)]
    public string BedsTypes { get; set; } = null!;

    [Required]
    public decimal PriceForNight { get; set; }
}