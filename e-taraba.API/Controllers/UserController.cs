using AutoMapper;
using e_taraba.API.DTOs;
using e_taraba.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace e_taraba.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IMapper mapper;
        private readonly IHash hash;

        public UserController(IRepository repository, IMapper mapper, IHash hash)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.hash = hash;
        }

        [HttpPost]
        public async Task<ActionResult<UserAuthenticationDto>> CreateUser([FromBody] UserAuthenticationDto request)
        {
            if (await repository.UserExistsASync(request.Username))
            {
                return Unauthorized("Username is taken");
            }


            hash.Generate(request.Password, out byte[] hashedPass, out byte[] hashSalt);

            request.Username = request.Username.Trim().ToLower();

            await repository.CreateUserASync(new Entities.User
            {
                Username = request.Username,
                hashedPassword = hashedPass,
                hashSalt = hashSalt
            });
            await repository.SaveChangesASync();

            return Ok(request);
        }

        [HttpDelete("{username}")]
        public async Task<ActionResult> DeleteUser(string username)
        {
            var userToDelete = await repository.GetUserByUsernameASync(username, false);
            if (userToDelete is null)
            {
                return NotFound();
            }
            repository.DeleteUserASync(userToDelete);
            await repository.SaveChangesASync();

            return NoContent();
        }
    }
}
