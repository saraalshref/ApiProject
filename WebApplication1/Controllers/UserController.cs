using Api_Project.Models;
using Api_Project.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Api_Project.DTOS;
namespace APIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration, IMapper mapper, IUserRepository userRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(UserRegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralResponse<string>(false, "Invalid data", null));
            }
            ApplicationUser existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return Conflict(new GeneralResponse<string>(false, "Email already exists", null));
            }
            ApplicationUser user = _mapper.Map<ApplicationUser>(model);
            user.UserName = model.Email;

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new GeneralResponse<string>(true, "User registered successfully", null));
            }

            string errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return BadRequest(new GeneralResponse<string>(false, errors, null));
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDTO loginUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data", ispass = false });
            }

            ApplicationUser user = await _userManager.FindByEmailAsync(loginUser.Email);
            if (user != null)
            {
                bool found = await _userManager.CheckPasswordAsync(user, loginUser.Password);
                if (found)
                {
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    SymmetricSecurityKey signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecKey"]));
                    SigningCredentials signingCredentials = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIss"],
                        audience: _configuration["JWT:ValidAud"],
                        expires: DateTime.Now.AddDays(2),
                        claims: claims,
                        signingCredentials: signingCredentials
                    );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expired = token.ValidTo,
                        ispass = true
                    });
                }
            }

            return Unauthorized(new
            {
                message = "Invalid email or password",
                ispass = false
            });
        }

        [HttpGet("users")]
        public async Task<ActionResult> GetUsers()
        {
            List<ApplicationUser> users = await _userRepository.GetAllUsersAsync();

            List<UserDTO> userDtos = _mapper.Map<List<UserDTO>>(users);

            return Ok(new GeneralResponse<List<UserDTO>>(true, "Users retrieved successfully", userDtos));
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateUser(string id, UserDTO updateUserDto)
        {

            ApplicationUser user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new GeneralResponse<string>(false, "User not found", null));
            }

            _mapper.Map(updateUserDto, user);

            await _userRepository.UpdateUserAsync(user);

            return Ok(new GeneralResponse<string>(true, "User updated successfully", null));
        }

        [HttpGet("getUserById/{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(string id)
        {
            ApplicationUser user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new GeneralResponse<string>(false, "User not found", null));
            }

            UserDTO userDto = _mapper.Map<UserDTO>(user);
            return Ok(new GeneralResponse<UserDTO>(true, "User retrieved successfully", userDto));
        }
    }
}
