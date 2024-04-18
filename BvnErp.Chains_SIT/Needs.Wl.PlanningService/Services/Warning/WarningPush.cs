using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services
{
    public class WarningPush
    {
        public List<WarningContext> SendContext { get; set; }

        public WarningPush(List<WarningContext> context)
        {
            this.SendContext = context;
        }

        public void PushWarning(int IntervalTime)
        {
            foreach (var item in this.SendContext)
            {
                var config = FrequencyConfig.Current.WarningFrequency.Where(t => t.Type == (int)item.NoticeType).FirstOrDefault();
                if (config != null)
                {
                    if(config.Frequency== IntervalTime)
                    {
                        if((config.WarningMethod & (int)WarningMethod.Mail) != 0)
                        {
                            SendMail(item);
                        }

                        if ((config.WarningMethod & (int)WarningMethod.Msg) != 0)
                        {
                            SendMsg(item);
                        }
                    }
                }
            }
        }

        private void SendMail(WarningContext item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.Email))
                {
                    XDTEmailService.Current.Send(item.Email, item.Title, item.Context);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SendMsg(WarningContext item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.Moblie))
                {
                    SMSService.Current.SendMessage(item.Moblie, item.Context);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

    }
}
