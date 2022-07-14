using System.ComponentModel.DataAnnotations.Schema;

namespace e_taraba.API.Entities
{
    [Table("tUsers", Schema = "taraba")]
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] hashedPassword { get; set; }
        public byte[] hashSalt { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
