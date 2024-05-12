using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; // This import seems unnecessary, as it's not being used in the code snippet.
using ChurchHub.Utils; 

using ChurchHub.Repository;

namespace ChurchHub.Repository
{
    public class IntentionManager
    {
        ChurchConnectEntities _db;
        BaseRepository<Intention> _intention; 
        AccountManager _accMgr;
        BaseRepository<Payment> _payment;

        // Constructor initializing necessary dependencies.
        public IntentionManager()
        {
            _db = new ChurchConnectEntities(); // Initializing dbContext.
            _intention = new BaseRepository<Intention>(); // Initializing repository for Intention entity.
            _accMgr = new AccountManager(); // Initializing AccountManager for user-related operations.
            _payment = new BaseRepository<Payment>();
        }

        // Method Retrieve list of Intentions by userId.
        public List<Intention> GetIntentionsByUserId(string userId)
        {
            return _intention._table.Where(m => m.userId == userId).ToList();
        }

        // Method to Retrieve an Intention by userId.
        public Intention GetIntentionByUserId(string userId)
        {
            return _intention._table.Where(m => m.userId == userId).FirstOrDefault();
        }

        public Intention GetIntentionByGuid(string intentionGUId)
        {
            return _intention._table.Where(m => m.intentionGUID == intentionGUId).FirstOrDefault();
        }

        // Update an Intention and return ErrorCode.
        public ErrorCode UpdateIntention(Intention intent, ref string errMsg)
        {
            return _intention.Update(intent.intentionId, intent, out errMsg);
        }

        // Create an Intention and return ErrorCode.
        public ErrorCode CreateIntention(Intention intent, string username, ref string errMsg)
        {
            // Get the user information based on the provided username.
            var user = _accMgr.GetUserInfoByUsername(username);

            // Assign userId and set intentionStatus to Pending.
            intent.intentionGUID = Utilities.gUid;
            intent.userId = user.userId;
            intent.intentionStatus = (int)IntentionStatus.Pending;

            // Attempt to create the Intention; return ErrorCode.
            if (_intention.Create(intent, out errMsg) != ErrorCode.Success)
            {
                return ErrorCode.Error;
            }

            return ErrorCode.Success;
        }

        public Intention GetIntentionByUsername(String username)
        {
            var user = _accMgr.GetUserByUsername(username);
            return _intention._table.Where(m => m.intentionGUID == user.UserId).FirstOrDefault();
        }

        // Method Retrieve an Intention by its id.
        public Intention GetIntentionById(int id)
        {
            return _intention.Get(id);
        }

        // Method  Retrieve an Intention by its id and return corresponding user's Intention.
        public Intention Retrieve(int id, ref string err)
        {
            var intention = GetIntentionById(id); // Get Intention by id.

            // Retrieve Intention based on user's userId.
            return GetIntentionByUserId(intention.userId);
        }


    }
}
