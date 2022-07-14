using System.ComponentModel.DataAnnotations.Schema;

namespace e_taraba.API.Entities
{
    [Table("tRefreshTokens", Schema = "taraba")]
    public class RefreshToken
    {
        public int Id { get; set; }
        public string RToken { get; set; }
        public bool Valid { get; set; }
        [Column("dateCreated")]
        public DateTime Created { get; set; }
        [Column("dateExpires")]
        public DateTime Expires { get; set; }
        [Column("idUser")]
        public int UserId { get; set; }
        public User User { get; set; }

    }
}
