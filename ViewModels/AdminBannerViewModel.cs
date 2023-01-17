using GCRBA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GCRBA.ViewModels
{
    public class AdminBannerViewModel
    {
        public User CurrentUser { get; set; }
        public MainBanner MainBanner { get; set; }
        public List<MainBanner> MainBanners { get; set; }
        public List<Image> ExistingNewsletters { get; set; }
        public Image CurrentNewsletter { get; set; }
        public String strCurrentNewsletterSource { get; set; }  

        public int sendCurrentNewsletter(ViewModels.AdminBannerViewModel adminBannerViewModel) {
            int sendConfirmation = 0;
            try {
                Models.Database db = new Models.Database();
                List<Models.User> userList = new List<Models.User>();
                userList = db.GetSubscriberEmailList();
                foreach (User user in userList) {
                    Models.SendNewsletterRequest newsletterRequest = new Models.SendNewsletterRequest();
                    newsletterRequest.Recipient = user.Email;
                    newsletterRequest.Image = adminBannerViewModel.CurrentNewsletter;
                    newsletterRequest.FirstName = user.FirstName;
                    OutgoingEmail.SendNewsletter(newsletterRequest);
                }
                sendConfirmation = 1;
                return sendConfirmation;
            }
            catch { }
            return sendConfirmation;
		}
    }
}