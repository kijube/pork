using System.ComponentModel.DataAnnotations;

namespace Pork.Manager;

public class SetNicknameRequest {
    [Required] [MaxLength(16)] public required string Nickname { get; set; }
}