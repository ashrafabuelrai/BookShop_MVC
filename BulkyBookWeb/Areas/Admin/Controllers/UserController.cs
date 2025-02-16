using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> _roleManager;
        [BindProperty]
        public RoleManagmentVM roleManagmentVM { get; set; }
        public UserController(IUnitOfWork unitOfWork, 
            Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager, 
            Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager)
        {
            
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult RoleManagment(string id)
        {
            roleManagmentVM = new()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.Get(a => a.Id == id,"Company"),
                RoleList= _roleManager.Roles.Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Name
                }),
                CompanyList= _unitOfWork .Company.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
            };
            roleManagmentVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(a => a.Id == id))
                .GetAwaiter().GetResult().FirstOrDefault();
            return View(roleManagmentVM);
        }
        [HttpPost]
        public IActionResult RoleManagment()
        {
            string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(a => a.Id == roleManagmentVM.ApplicationUser.Id))
                .GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser userDb = _unitOfWork.ApplicationUser.Get(a => a.Id == roleManagmentVM.ApplicationUser.Id);

            if (roleManagmentVM.ApplicationUser.Role != oldRole)
            {
           
                if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
                {
                    userDb.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
                if(oldRole==SD.Role_Company)
                {
                    userDb.CompanyId = null;
                }
                _unitOfWork.ApplicationUser.Update(userDb);
                _unitOfWork.Save();
                _userManager.RemoveFromRoleAsync(userDb, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(userDb, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }
            else
            {
                if (oldRole == SD.Role_Company&&userDb.CompanyId!=roleManagmentVM.ApplicationUser.CompanyId)
                {
                    userDb.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.Update(userDb);
                    _unitOfWork.Save();
                }
            }
           
            return RedirectToAction(nameof(Index));
        }
        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _unitOfWork.ApplicationUser.GetAll(null,"Company").ToList();
            
            foreach(var user in userList)
            {
                
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
                if (user.Company==null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            return Json(new { data = userList });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDb = _unitOfWork.ApplicationUser.Get(a => a.Id == id);
            if(objFromDb==null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if(objFromDb.LockoutEnd!=null&&objFromDb.LockoutEnd>DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _unitOfWork.ApplicationUser.Update(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Operation successful" });

        }
        #endregion
    }
}
