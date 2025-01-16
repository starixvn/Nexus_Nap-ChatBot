namespace Fanex.NCB.AutoJobs.Core.Tests.Services.TestableClasses
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.ResultAmendment;
    using Fanex.NCB.AutoJobs.Core.Services;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;

    public class ResultAmendmentServiceTestable : ResultAmendmentService
    {
        public ResultAmendmentServiceTestable(
            IBaseService baseService,
            IChatbotService chatbotService,
            ICommonService commonService
        ) : base(baseService, chatbotService, commonService)
        {
        }

        public async Task Public_ImportResultAmendmentInternalCSDAsync(IEnumerable<MessageAmendmentCsd> messages, DateTime? newLastModifiedTime)
        {
            await ImportResultAmendmentInternalCSDAsync(messages, newLastModifiedTime);
        }

        public async Task Public_ImportResultAmendmentInternalLicenseeAsync(
            IEnumerable<MessageAmendmentHeader> messageHeaders,
            IEnumerable<MessageAmendmentDetail> messageDetails,
            IEnumerable<MessageAmendmentTarget> messageTargets
        )
        {
            await ImportResultAmendmentInternalLicenseeAsync(messageHeaders, messageDetails, messageTargets);
        }
    }
}
