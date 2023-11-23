//namespace Fanex.NCB.Repository
//{
//    using System.Collections.Generic;
//    using System.Threading.Tasks;
//    using Fanex.Data;
//    using Fanex.Data.Repository;
//    using Fanex.NCB.Repository.Interfaces;
//    using Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets;

//    public class SpecialDeleteRepository : ISpecialDeleteRepository
//    {
//        public async Task<(IEnumerable<AbnormalMessageDetail>, IEnumerable<AbnormalMessage>, IEnumerable<AbnormalAccountInfo>, IEnumerable<DuplicateTicketDetail>)>
//        FetchVoidingHistoryInfo(ICriteria criteria)
//        {
//            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
//            {
//                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
//                {
//                    var data = new VoidingHistoryInfo
//                    {
//                        AbnormalMessageDetail = multi.Read<AbnormalMessageDetail>(),
//                        AbnormalMessage =  multi.Read<AbnormalMessage>(),
//                        AbnormalAccountInfo = multi.Read<AbnormalAccountInfo>(),
//                        DuplicateTicketDetail = multi.Read<DuplicateTicketDetail>()
//                    };

//                    return (data.AbnormalMessageDetail, data.AbnormalMessage, data.AbnormalAccountInfo, data.DuplicateTicketDetail);
//                }
//            }
//        }
//    }
//}