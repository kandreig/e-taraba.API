namespace e_taraba.API.DTOs
{
    public class RefreshTokenDto
    {
        public string RToken { get; set; }
        public bool Valid { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public int UserId { get; set; }
    }
}
