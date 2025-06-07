using FoodioAPI.Database;
using FoodioAPI.DTOs.UserDtos;
using FoodioAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodioAPI.Controllers
{
    [Route("api/admin/employees")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class EmployeesController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public EmployeesController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = _context.Users
                //.Where(u => u.Role == "Shipper" || u.Role == "Kitchen" || u.Role == "Cashier")
                .Select(u => new EmployeeDto
                {
                    Id = u.Id,
                    UserName = u.UserName!,
                    Email = u.Email!,
                    //Role = u.Role
                })
                .ToList();

            return Ok(employees);
        }

        [HttpPost("shifts")]
        public async Task<ActionResult<ShiftDto>> CreateShift([FromBody] CreateShiftDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found");

            if (!new[] { "Shipper", "Kitchen", "Cashier" }.Contains(dto.Role))
                return BadRequest("Invalid role");

            var shift = new Shift
            {
                UserId = dto.UserId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Role = dto.Role
            };

            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            var shiftDto = new ShiftDto
            {
                Id = shift.Id,
                UserId = shift.UserId,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Role = shift.Role
            };

            return CreatedAtAction(nameof(GetShift), new { id = shift.Id }, shiftDto);
        }

        [HttpGet("shifts/{id}")]
        public async Task<ActionResult<ShiftDto>> GetShift(Guid id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null)
                return NotFound("Shift not found");

            var shiftDto = new ShiftDto
            {
                Id = shift.Id,
                UserId = shift.UserId,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Role = shift.Role
            };

            return Ok(shiftDto);
        }

        [HttpGet("shifts")]
        public async Task<ActionResult<IEnumerable<ShiftDto>>> GetShifts()
        {
            var shifts = _context.Shifts
                .Select(s => new ShiftDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Role = s.Role
                })
                .ToList();

            return Ok(shifts);
        }

        [HttpDelete("shifts/{id}")]
        public async Task<IActionResult> DeleteShift(Guid id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null)
                return NotFound("Shift not found");

            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
