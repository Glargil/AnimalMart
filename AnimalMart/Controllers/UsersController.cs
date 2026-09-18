using AnimalMart.Repos;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace AnimalMart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepo _repo;
        //constructor to receive the repository instance
        public UsersController(IUserRepo repo)
        {
            _repo = repo;
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
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // POST api/<UsersController>
        [HttpPost]
        public ActionResult<User?> Post([FromBody] User? user)
        {
            if (user == null)
            {
                return BadRequest("User data is required.");
            }
            else
            {
                _repo.CreateUser(user);
                //returns the object AND a string to give our new resource a URI for the header in the response
                return Created($"api/users/{user?.Id}", user);
            }

        }
    }
}
