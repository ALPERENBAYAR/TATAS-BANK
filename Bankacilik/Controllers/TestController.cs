using Microsoft.AspNetCore.Mvc;

public class TestController : Controller
{
    [HttpGet("hash-test")]
    public IActionResult GetHashedPassword()
    {   
        var hashedPassword = PasswordHasher.HashPassword("123");
        return Ok(hashedPassword);
    }
}
