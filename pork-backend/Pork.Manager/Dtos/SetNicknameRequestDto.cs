using System.ComponentModel.DataAnnotations;

namespace Pork.Manager.Dtos;

public class SetNicknameRequestDto {
    [Required] [MaxLength(16)] public required string Nickname { get; set; }
}