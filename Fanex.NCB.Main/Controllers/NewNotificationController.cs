using Fanex.NAP.Client.AspNetCore.Attributes;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NCB.Core.Constants;
using Fanex.NCB.Main.ViewModel.Notification;
using Fanex.NCB.Main.ViewModel.TreeSelect;
using Fanex.NCB.Models.Notification;
using Fanex.NCB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Fanex.NAP.Core.Business.Constant;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [NapSuperAuth]
    public class NewNotificationController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly INapSession napSession;

        public NewNotificationController(INotificationService notificationService, INapSession napSession)
        {
            this.notificationService = notificationService;
            this.napSession = napSession;
        }

        [NapAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/NewNotification/Index.cshtml");
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        public async Task<IActionResult> Create(CreateNewNotificationViewModel model)
        {
            var result = await notificationService.CreateAsync(new CreateNotification
            {
                NotificationTypeId = model.NotificationTypeId,
                ProductId = model.ProductId,
                RecipientIds = model.RecipientIds,
                Title = model.Title,
                Content = model.Content,
                SendDate = model.SendDate,
                SendImmediately = model.SendImmediately,
                CreatorId = napSession.UserId
            }).ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        [NapAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        public async Task<IActionResult> GetRecipients(int notificationTypeId, int? productId)
        {
            var result = await notificationService.GetRecipientsAsync(notificationTypeId, productId).ConfigureAwait(false);

            var tree = GenerateTree(result);

            return Json(tree);
        }

        private IEnumerable<TreeNodeItem> GenerateTree(IEnumerable<Recipient> data)
        {
            return data.GroupBy(x => new { x.RecipientTypeId, x.RecipientTypeName }, x => x, (key, list) =>
            {
                var children = list.Where(x => !x.LicenseeTypeId.HasValue).Select(x => new TreeNodeItem
                {
                    Title = x.RecipientName,
                    Value = $"{x.RecipientTypeId}-{x.RecipientId}",
                    RealValue = x.RecipientId,
                }).ToList();

                children.AddRange(list.Where(x => x.LicenseeTypeId.HasValue).GroupBy(x => new { x.LicenseeTypeId, x.LicenseeTypeName },
                    x => x,
                    (licensee, recipients) =>
                    {
                        return new TreeNodeItem
                        {
                            Title = licensee.LicenseeTypeName,
                            Value = $"{key.RecipientTypeId}-{licensee.LicenseeTypeId}",
                            RealValue = licensee.LicenseeTypeId.Value,
                            Children = recipients.Select(x => new TreeNodeItem
                            {
                                Title = x.RecipientName,
                                Value = $"{key.RecipientTypeId}-{licensee.LicenseeTypeId}-{x.RecipientId}",
                                RealValue = x.RecipientId
                            })
                        };
                    }));

                return new TreeNodeItem
                {
                    Title = key.RecipientTypeName,
                    Value = $"{key.RecipientTypeId}",
                    RealValue = key.RecipientTypeId,
                    Children = children
                };
            });
        }
    }
}