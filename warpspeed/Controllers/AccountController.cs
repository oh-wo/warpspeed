using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Routing;
using System.Web.Security;
using warpspeed.Models;
using warpspeed.Helpers;


namespace warpspeed.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //
        // GET: /Account/LogOn
        [AllowAnonymous]
        public ActionResult LogOn(string returnUrl)
        {
            //So that the user can be referred back to where they were when they click logon
            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult LogOn(string Email, string Password)
        {
            LogOnSuccess logOn = new LogOnSuccess()
            {
                Success = false,
            };
            WUser user = new WUser();
            if ((user = AuthenticationHelper.ValidateUser(Email, Password)) != null)
            {
                if (AuthenticationHelper.LogInUser(user))
                {
                    logOn.Success = true;
                    using (warpspeedEntities db = new warpspeedEntities())
                    {
                        WUser thisUser = db.WUsers.First(u => u.Email == Email);
                        logOn.Name = String.Format("Hi {0} {1}", thisUser.FirstName, thisUser.LastName);
                    }
                }
                else
                {
                    logOn.ErrorMessage = "Unknown error. Try reloading the page.";
                }
            }
            else
            {
                logOn.ErrorMessage = "Incorrect email / password combination";
            }
            return Json(logOn);
        }

        public class LogOnModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        private class LogOnSuccess
        {
            public bool Success { get; set; }
            public string Name { get; set; }
            public string ErrorMessage { get; set; }
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();


            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public JsonResult JsonLogOff()
        {
            var success = false;
            try
            {
                FormsAuthentication.SignOut();
                success = true;
            }
            catch (Exception ex)
            {
            }
            return Json(success);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            bool success = false;
            using (warpspeedEntities db = new warpspeedEntities())
            {
                if (db.WUsers.Any(u => u.Email == model.Email))
                {
                    return View("EmailAlreadyExists");
                }
                else
                {
                    // Attempt to register the user
                    AuthenticationHelper authentication = new AuthenticationHelper();
                    try
                    {
                        WUser user = new WUser();
                        user = null;

                        Guid? userID = AuthenticationHelper.CreateUser(model.Email, model.Password, model.LastName, model.FirstName);

                        user = db.WUsers.First(u => u.ID == userID);
                        AuthenticationHelper.LogInUser(user);
                        success = true;

                    }
                    catch (Exception ex)
                    {
                    }

                    return Json(success);
                }
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult IncorrectRegToken()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult EmailSent()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult EmailAlreadyExists()
        {
            return View();
        }

        public class RegisterModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            // ChangePassword will throw an exception rather
            // than return false in certain failure scenarios.
            bool changePasswordSucceeded;
            try
            {
                MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
            }
            catch (Exception)
            {
                changePasswordSucceeded = false;
            }

            if (changePasswordSucceeded)
            {
                return RedirectToAction("ChangePasswordSuccess");
            }
            else
            {

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public class ChangePasswordModel
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        [AllowAnonymous]
        public ActionResult NotAllowedUser()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult registrationConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult VerifyEmail(Guid emailCode)
        {
            bool success = false;
            if (emailCode != null)
            {
                try
                {
                    using (warpspeedEntities db = new warpspeedEntities())
                    {
                        if (db.WUsers.Any(u => u.EmailCode == emailCode))//This isn't legit. Really need to get the userID from the page somehow??!
                        {                     //Currently just assuming that the user who clicked this link has the rights. ie mobile checks mobile no && smsCode
                            WUser user = db.WUsers.First(u => u.EmailCode == emailCode);
                            success = true;
                            user.EmailVerified = true;
                            //reset the user's email code so can't be used again
                            user.EmailCode = Guid.NewGuid();
                        }


                        db.SaveChanges();
                        success = true;
                    }

                }
                catch (Exception ex)
                {
                    return View("VerificationFailed");
                }
            };
            if (success)
            {

                return View("VerificationSuccess");
            }
            else
            {
                return View("VerificationFailed");
            };
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult VerificationSuccess()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult VerificationFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult _PleaseLoginRegister()
        {
            return View();
        }
    }
}
