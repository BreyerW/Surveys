using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Surveys;
using Surveys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Surveys.Controllers
{
    public class AccountController : Controller
    {
        private readonly surveyContext _context;
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService, surveyContext context)
        {
            _authService = authService;
            _context = context;
        }
        // GET: AccountController
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] LoginData login)
        {
            if (ModelState.IsValid)
            {
                var validUser = _authService.Authenticate(login.Username, login.Password);
                if (validUser == null)
                    ModelState.AddModelError("Password", "Nazwa użytkownika lub hasło są niepoprawne");
                else
                {
                    var claims = new List<Claim>();
                    var role = _context.Roles.SingleOrDefault(r => r.Id == validUser.IdRole).RoleName;
                    claims.Add(new Claim(ClaimTypes.Name, validUser.Username));
                    claims.Add(new Claim(ClaimTypes.Role, role));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, validUser.Id.ToString()));
                    var userIdentity = new ClaimsIdentity("SuperSecureLogin");
                    userIdentity.AddClaims(claims);
                    var userPrincipal = new ClaimsPrincipal(userIdentity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal,
                    new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                        IsPersistent = false,
                        AllowRefresh = false
                    });
                    return RedirectToAction(nameof(Index), "Home");
                }
            }
            return View(login);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Index), "Home");
        }
        // GET: Account/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            //ViewData["IdRole"] = new SelectList(_context.Roles, "Id", "Id");
            return View();
        }

        // POST: Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Sex,BirthYear,Password,ConfirmPassword")] User user)
        {
            const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
            const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
            const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
            const int SaltSize = 128 / 8; // 128 bits
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Username == user.Username) is false)
                {
                    user.IdRole = _context.Users.Any() ? 2 : 1;

                    byte[] salt = new byte[128 / 8];
                    using (var rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(salt);
                    }

                    var subkey = KeyDerivation.Pbkdf2(
                         password: user.Password,
                         salt: salt,
                         prf: Pbkdf2Prf,
                         iterationCount: Pbkdf2IterCount,
                         numBytesRequested: Pbkdf2SubkeyLength);
                    var outputBytes = new byte[SaltSize + Pbkdf2SubkeyLength];
                    Buffer.BlockCopy(salt, 0, outputBytes, 0, SaltSize);
                    Buffer.BlockCopy(subkey, 0, outputBytes, SaltSize, Pbkdf2SubkeyLength);
                    var hashed = Convert.ToBase64String(outputBytes);
                    user.Password = hashed;

                    subkey = KeyDerivation.Pbkdf2(
                         password: user.BirthYear,
                         salt: salt,
                         prf: Pbkdf2Prf,
                         iterationCount: Pbkdf2IterCount,
                         numBytesRequested: Pbkdf2SubkeyLength);
                    outputBytes = new byte[SaltSize + Pbkdf2SubkeyLength];
                    Buffer.BlockCopy(salt, 0, outputBytes, 0, SaltSize);
                    Buffer.BlockCopy(subkey, 0, outputBytes, SaltSize, Pbkdf2SubkeyLength);
                    hashed = Convert.ToBase64String(outputBytes);
                    user.BirthYear = hashed;

                    subkey = KeyDerivation.Pbkdf2(
                         password: user.Sex,
                         salt: salt,
                         prf: Pbkdf2Prf,
                         iterationCount: Pbkdf2IterCount,
                         numBytesRequested: Pbkdf2SubkeyLength);
                    outputBytes = new byte[SaltSize + Pbkdf2SubkeyLength];
                    Buffer.BlockCopy(salt, 0, outputBytes, 0, SaltSize);
                    Buffer.BlockCopy(subkey, 0, outputBytes, SaltSize, Pbkdf2SubkeyLength);
                    hashed = Convert.ToBase64String(outputBytes);
                    user.Sex = hashed;

                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Success));
                }
                else
                    ModelState.AddModelError("Username", "Ta nazwa jest już zajęta");
            }
            //ViewData["IdRole"] = new SelectList(_context.Roles, "Id", "Id", user.IdRole);
            return View(user);
        }
        // GET: Users/Success
        [AllowAnonymous]
        public IActionResult Success()
        {
            
            return View();
        }
    }
}
