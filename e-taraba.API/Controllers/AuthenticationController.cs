using AutoMapper;
using e_taraba.API.DTOs;
using e_taraba.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace e_taraba.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IMapper mapper;
        private readonly IHash hash;
        private readonly IConfiguration configuration;

        public AuthenticationController(IRepository repository, IMapper mapper, IHash hash, IConfiguration configuration)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.hash = hash;
            this.configuration = configuration;
        }


        [HttpPost]
        public async Task<ActionResult> Login([FromBody] UserAuthenticationDto request)
        {
            request.Username = request.Username.Trim().ToLower();

            if (!await repository.UserExistsASync(request.Username))
            {
                return Unauthorized("username is not valid");
            }
            var userFromDb = await repository.GetUserByUsernameASync(request.Username, true);

            if (!hash.Validate(request.Password, userFromDb.hashedPassword, userFromDb.hashSalt))
            {
                return Unauthorized("Incorrect password");
            }

            var userForClaimsDto = mapper.Map<UserForClaimsDto>(userFromDb);

            var userToReturn = mapper.Map<UserInfoDto>(userFromDb);
            var accesToken = CreateAccesToken(userForClaimsDto);
            var rToken = await GenerateRToken(userForClaimsDto);


            var informationToReturn = new { access_token = accesToken, refresh_token = rToken, user = userToReturn };
            return Ok(informationToReturn);
        }


        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody]string refreshTokenToInvalidate)
        {
            await InvalidateRefreshToken(refreshTokenToInvalidate);

            return NoContent();
        }


        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken([FromBody] string RToken)
        {

            var RTokenFromDb = await repository.GetRefreshTokenByStringASync(RToken);

            if(RTokenFromDb == null || RTokenFromDb.Expires < DateTime.UtcNow)
            {
                return Unauthorized("Invalid token request. Please login again");
            }

            var userOfToken = await repository.GetUserASync(RTokenFromDb.UserId, true);

            if (RTokenFromDb.Valid == false)
            {
                var RTokenInvalidate = new RefreshTokenUpdateDto { Valid = false };

                foreach (var rtoken in userOfToken.RefreshTokens)
                {
                    mapper.Map(RTokenInvalidate, rtoken);
                }
                await repository.SaveChangesASync();

                return Unauthorized("Fraud detected, loggin out user");
            }

            var userForClaims = mapper.Map<UserForClaimsDto>(userOfToken);

            var createdAccesToken = CreateAccesToken(userForClaims);
            var createdRefreshToken = await GenerateRToken(userForClaims,RToken);


            var informationToReturn = new { access_token = createdAccesToken, refresh_token = createdRefreshToken, username = userOfToken.Username};

            return Ok(informationToReturn);
        }

        [Authorize]
        [HttpPost("check-atoken")]
        public async Task<ActionResult> CheckAccessToken()
        {
            return Ok("Access token is valid");
        }

        private async Task<string> GenerateRToken(UserForClaimsDto user, string? oldRToken = null)
        {

            if(oldRToken != null)
            {
                await InvalidateRefreshToken(oldRToken);
            }

            var rToken = new RefreshTokenDto
            {
                RToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Valid = true,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMonths(6),
                UserId = user.Id
            };

            await repository.CreateRefreshTokenASync(mapper.Map<Entities.RefreshToken>(rToken));
            await repository.SaveChangesASync();

            return rToken.RToken;

        }


        private async Task InvalidateRefreshToken(string oldRToken)
        {
            var RTokenInvalidate = new RefreshTokenUpdateDto { Valid = false };
            var RTokenFromDb = await repository.GetRefreshTokenByStringASync(oldRToken);

            mapper.Map(RTokenInvalidate, RTokenFromDb);
            await repository.SaveChangesASync();
        }



        //create token
        private string CreateAccesToken(UserForClaimsDto userClaimsDto)
        {
            var securityKey = new SymmetricSecurityKey(
             System.Text.Encoding.UTF8.GetBytes(configuration["Authentication:SecretForKey"])
             );
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("name", userClaimsDto.Username));

            var jwtSecurityToken = new JwtSecurityToken(
                configuration["Authentication:Issuer"],
                configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return tokenToReturn;
        }
    }
}
