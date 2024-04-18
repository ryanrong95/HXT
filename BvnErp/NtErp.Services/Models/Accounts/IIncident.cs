using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Models
{
    public interface IIncident
    {
        event SuccessHanlder EnterSuccess;
        event ErrorHanlder EnterError;
    }
    public interface IPayment : IIncident
    {
        void Enter();
    }
    public interface IRecharge : IIncident
    {
        void Enter();
    }

    public interface IRefund : IIncident
    {
        void Enter();
    }
}
