namespace e_taraba.API.Services
{
    public interface IHash
    {
        void Generate(string password, out byte[] hashPassword, out byte[] hashSalt);
        bool Validate(string password, byte[] hashPassword, byte[] hashSalt);
    }
}
