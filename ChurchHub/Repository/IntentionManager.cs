using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChurchHub.Utils;
using ChurchHub.Repository;

namespace ChurchHub.Repository
{
    public class IntentionManager
    {

        ChurchHubEntities _db;
        BaseRepository<Intention> _intention;
        AccountManager _accMgr;



        public IntentionManager()
        {
            _db = new ChurchHubEntities();
            _intention = new BaseRepository<Intention>();
            _accMgr = new AccountManager();
        }

        public Intention GetIntentionByUserId(String userId)
        {
            return _intention._table.Where(m => m.userId == userId).FirstOrDefault();
        }
        public ErrorCode UpdateIntention(Intention intent, ref String errMsg)
        {
            return _intention.Update(intent.intentionId, intent, out errMsg);
        }

        public ErrorCode CreateIntention(Intention intent, string username, ref string errMsg)
        {
            // Get the user information
            var user = _accMgr.GetUserInfoByUsername(username);

            intent.userId = user.userId;            
            intent.intentionStatus = (int)IntentionStatus.Pending;
            // If creation succeeds, return ErrorCode.Success
            // Attempt to create the new intention in the database
            if (_intention.Create(intent, out errMsg) != ErrorCode.Success)
            {           
                
                return ErrorCode.Error;
            }           

            return ErrorCode.Success;
        }
        public Intention CreateIntentionByUserId(String username, ref String err)
        {

            var user = _accMgr.GetUserInfoByUsername(username);
            var intention = GetIntentionByUserId(user.userId);

            

            intention = new Intention();
            intention.userId = user.userId;
            intention.intentionStatus = (Int32)IntentionStatus.Pending;

            _intention.Create(intention, out err);

            return GetIntentionByUserId(user.userId);
        }
    }
}