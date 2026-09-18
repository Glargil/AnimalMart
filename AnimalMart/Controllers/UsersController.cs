using AnimalMart.Interfaces;
using AnimalMart.Models;
using AnimalMart.Services;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace AnimalMart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepo _repo;
        private readonly IUserService _userService;
        //constructor to receive the repository instance
        public UsersController(IUserRepo repo, IUserService userService)
        {
            _repo = repo;
            _userService = userService;
        }

        // GET: api/<UsersController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            IEnumerable<User>? result = _repo.GetAllUsers();
            if (result == null || !result.Any())
            {
                return NoContent();
            }
            return Ok(result.Select(u => new UserDTO(u)));
        }

        // POST: api/<UsersController>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public ActionResult<UserDTO> Post([FromBody] UserCreateDTO? dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Name, email, and password are required.");
            }

            var user = _userService.CreateUser(dto);
            var result = new UserDTO(user);
            return Created($"api/users/{result.Id}", result);
        }
    }
}
