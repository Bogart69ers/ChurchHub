using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChurchHub.Utils;

namespace ChurchHub.Repository
{
    public class PaymentManager
    {

        BaseRepository<Payment> _payment;
        IntentionManager _intention;
        AccountManager _accMgr;
        BaseRepository<Intention> _intentions;



        public PaymentManager()
        {
            _payment = new BaseRepository<Payment>();
            _intention = new IntentionManager(); // Initializing repository for Intention entity.
            _accMgr = new AccountManager(); // Initializing AccountManager for user-related operations.
            _intentions = new BaseRepository<Intention>(); // Initializing repository for Intention entity.

        }

        public ErrorCode CreatePayment(Payment p, string intentionGUID, ref string errMsg)
        {
            var intention = _intention.GetIntentionByGuid(intentionGUID);

            // Assign the intention ID
            p.intentionId = intention.intentionGUID;
            p.PaymentStatus = (int)PaymentStatus.Paid;
            p.PaymentDate = DateTime.Now;

            // Create the payment
            if (_payment.Create(p, out errMsg) != ErrorCode.Success)
            {
                return ErrorCode.Error;
            }

            // Update the intention with the payment ID
            intention.paymentId = p.paymentId;

            if (_intentions.Update(intention.intentionId,intention, out errMsg) != ErrorCode.Success)
            {
                // Rollback the payment creation if updating the intention fails
                _payment.Delete(p.paymentId, out errMsg); // Assuming there's a Delete method to remove the payment
                return ErrorCode.Error;
            }

            return ErrorCode.Success;
        }





    }
}