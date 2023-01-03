﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using GCRBA.ViewModels;
using GCRBA.Models;
using GCRBA.Classes;

namespace GCRBA.Controllers
{
    public class UserController : Controller
    {
        Classes.ObfuscatePwd obfuscater = new ObfuscatePwd();
        //Database db = new Database();

        public ActionResult Index()
        {
            Models.User u = new Models.User();
            u = u.GetUserSession();

            return View(u);
        }

        [HttpPost]
        public ActionResult Index(FormCollection col)
        {
            try
            {
                Models.User u = new Models.User();
                u = u.GetUserSession();
                u.FirstName = col["FirstName"];
                u.LastName = col["LastName"];
                u.Email = col["Email"];
                //u.UserID = col["UserID"];
                u.strPassword = col["Password"];

                if (u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.Password.Length == 0)
                {
                    u.ActionType = Models.User.ActionTypes.RequiredFieldMissing;
                    return View(u);
                }

                else
                {
                    if (col["btnSubmit"] == "newuser")
                    { //sign up button pressed
                        Models.User.ActionTypes at = Models.User.ActionTypes.NoType;
                        at = u.Save();
                        switch (at)
                        {
                            case Models.User.ActionTypes.InsertSuccessful:
                                u.SaveUserSession();
                                // TODO: return to user profile page/index
                                return RedirectToAction("Home", "Index");
                            //break;
                            default:
                                return View(u);
                                //break;
                        }
                    }
                    else
                    {
                        return View(u);
                    }
                }
            }
            catch (Exception)
            {
                Models.User u = new Models.User();
                return View(u);
            }
        }

        public ActionResult AddNewUser()
        {
            Models.User u = new Models.User();
            return View(u);
        }

        [HttpPost]
        public ActionResult AddNewUser(FormCollection col)
        {
            try
            {
                Models.User u = new Models.User();

                // only using FirstName, LastName, Email, Username, and Password because
                // those are the only ones on the form for a new user 
                u.FirstName = col["FirstName"];
                u.LastName = col["LastName"];
                u.Email = col["Email"];
                u.strUsername = col["Username"];
                u.strPassword = col["strPassword"];
                
                // permissions control
                u.isMember = 0;
                u.isAdmin = 0;

                // some blank values to return to db
                u.Address = "";
                u.City = "";
                u.State = "";
                u.Zip = "";
                u.Phone = "";
                u.MemberShipType = "";

                // submit new user button pressed
                if (col["btnSubmit"].ToString() == "newuser")
                {
                    // initialize action type
                    Models.User.ActionTypes at = Models.User.ActionTypes.NoType;
                       
                    // make sure none of the fields are empty
                    if (u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.strUsername.Length == 0
                            || u.strPassword.Length == 0)
                    {
                        // empty field(s), access action type on view to display relevant error message
                        u.ActionType = Models.User.ActionTypes.RequiredFieldMissing;
                        return View(u);
                    }

                    // create database object 
                    Database db = new Database();

                    //Obfuscate username and password prior to saving
                    u.encryptedUsername = obfuscater.SimpleObfuscateCredentials(u.strUsername);
                    u = obfuscater.ComplexObfuscateCredentials(u.strPassword, u);

                    // save action type based on what Save() returns 
                    at = u.Save();

                    switch (at)
                    {
                        // redirect to interface based on member/nonmember
                        case Models.User.ActionTypes.InsertSuccessful:
                            u.SaveUserSession();
                            // check to see if user is a member or not 
                            db.IsUserMember(u);
                            if (u.isMember == 0)
                            {
                                return RedirectToAction("NonMember", "Profile");
                            }
                            else
                            {
                                return RedirectToAction("Member", "Profile");
                            }
                        default:
                            return View(u);
                    }
                }
                if (col["btnSubmit"].ToString() == "home")
                {
                    return RedirectToAction("Index", "Home");
                }
                return View(u);
                
            }
            catch (Exception)
            {
                Models.User u = new Models.User();
                return View(u);
            }
        }

        public ActionResult AddNewMember()
        {
            Models.User u = new Models.User() { }; 
            Models.Database db = new Models.Database();
            u = u.GetUserSession();
            u.lstStates = db.GetStates();
            return View(u);
        }

        [HttpPost]
        public ActionResult AddNewMember(FormCollection col)
        {
            try {

                // get current user session
                Models.User user = new Models.User();
                user = user.GetUserSession();                

                // contact info
                user.FirstName = col["FirstName"];
                user.LastName = col["LastName"];
                user.Address = col["Address"];
                user.userPhone = new Models.PhoneNumber();
                user.userPhone.AreaCode = col["userPhone.AreaCode"];
                user.userPhone.Prefix = col["userPhone.Prefix"];
                user.userPhone.Suffix = col["userPhone.Suffix"];
                user.Phone = col["userPhone.AreaCode"] + col["userPhone.Prefix"] + col["userPhone.Suffix"];
                user.City = col["City"];
                user.intState = Convert.ToInt16(col["intState"]);

                //Just doing something quick here... probably should be changed to something more dynamic.
                if (user.intState == 1) user.State = "Indiana";
                else if (user.intState == 2) user.State = "Kentucky";
                else if (user.intState == 3) user.State = "Ohio";
                user.Zip = col["Zip"];

                // set sign in info if user is new
                if (user.UID == 0) {
                    // username/pass setup
                    user.Email = col["Email"];
                    user.strUsername = col["strUsername"];
                    user.strPassword = col["strPassword"];
                    user.encryptedUsername = obfuscater.SimpleObfuscateCredentials(user.strUsername);
                    user = obfuscater.ComplexObfuscateCredentials(user.strPassword, user);
                }

                // membership type
                if (col["MemberShipType"] != null) {
                    if (col["MemberShipType"].ToString() == "Associate") {
                        user.intMembershipType = 1;
                        user.MemberShipType = "Associate";
                    }
                    else if (col["MemberShipType"].ToString() == "Business") {
                        user.intMembershipType = 2;
                        user.MemberShipType = "Business";
                    }
                    else if (col["MemberShipType"].ToString() == "Allied") {
                        user.intMembershipType = 3;
                        user.MemberShipType = "Allied";
                    }
				}

                if (col["PaymentType"] != null) {
                    if (col["PaymentType"].ToString() == "Zelle") {
                        user.intPaymentType = 1;
                        user.PaymentType = "Zelle";
                    }

                    else if (col["PaymentType"].ToString() == "Check") {
                        user.intPaymentType = 2;
                        user.PaymentType = "Check";
                    }
                }

                //permissions
                //user.isMember = 1;
                user.isAdmin = 0;

                // once submit is hit, process member data
                if (col["btnSubmit"] == "submit")
                {
                    //validate data
                    if (user.FirstName.Length == 0 || user.LastName.Length == 0 || user.Email.Length == 0 ||
                        user.Phone.Length == 0 || user.Address.Length == 0 || user.City.Length == 0 || user.State.Length == 0 ||
                        user.Zip.Length == 0 || user.intPaymentType == 0 || user.intMembershipType == 0)
                    {
                        // empty field(s), access action type on view to display relevant error message
                        user.ActionType = Models.User.ActionTypes.RequiredFieldMissing;
                        return View(user);
                    }

                    // send data if valid to db
                    else
                    {
                        // initialize action type
                        Models.User.ActionTypes at = Models.User.ActionTypes.NoType;
                            
                        // create database object 
                        Database db = new Database();

                        // if user is authenticated then update member table
                        if (user.UID > 0)
                        {

                            // update user data
                            at = db.UpdateUser_NotVM(user);

                            switch (at)
                            {
                                case Models.User.ActionTypes.UpdateSuccessful:
                                
                                    // send request to member table to be reviewed by admin
                                    db.InsertMemberRequest(user);

                                    // send Grace email and send to admin controller
                                    OutgoingEmail.SendMemberRequest(user);
                                    //SendMemberEmail.SendEmail(user);

                                    return RedirectToAction("Index", "Home");
                                default:
                                    user.lstStates = db.GetStates();
                                    return View(user);
                            }

                        }
                        else
                        {
                            // save action type based on what Save() returns 
                            at = user.Save();

                            switch (at)
                            {
                                // insert successful- redirect as necessary 
                                case Models.User.ActionTypes.InsertSuccessful:

                                    user.SaveUserSession();
                                    db.InsertMemberRequest(user);
                                    //SendMemberEmail.SendEmail(user);
                                    OutgoingEmail.SendMemberRequest(user);
                                    return RedirectToAction("Index", "Home");
                                
                                case Models.User.ActionTypes.DuplicateEmail:
                                    user.lstStates = db.GetStates();
                                    return View(user);

                                default:
                                    user.lstStates = db.GetStates();
                                    return View(user);
                            }                        
                        }
                    }
                }
            }
            catch (Exception)
            {
                Models.User u = new Models.User();
                Models.Database db = new Models.Database();
                u.lstStates = db.GetStates();
                return View(u);
            }
            return View();
        }

    }
}