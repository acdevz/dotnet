using CityCentralLibrary.DAL.Data;
using CityCentralLibrary.DAL.Models;

namespace CityCentralLibrary.DAL.Repositories
{
    public class Repository : IRepository
    {

        public IEnumerable<MemberInfo> GetAllMembers()
        {
            using var dbContext = new MemberInfoDbContext();
            return dbContext.MemberInfo.ToList();
        }

        public bool RegisterForMembership(MemberInfo member)
        {
            try
            {
                using var dbContext = new MemberInfoDbContext();
                dbContext.MemberInfo.Add(member);
                dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CancelMembership(int registrationId)
        {
            try
            {
                using var dbContext = new MemberInfoDbContext();
                var member = dbContext.MemberInfo.FirstOrDefault(m => m.RegistrationID == registrationId);
                if (member != null) { 
                    dbContext.Remove(member);
                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateEmail(int registrationId, string email)
        {
            try
            {
                using var dbContext = new MemberInfoDbContext();
                var member = dbContext.MemberInfo.FirstOrDefault(m => m.RegistrationID == registrationId);
                if(member != null)
                {
                    member.EmailID = email;
                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<MemberInfo> SearchMember(string searchTerm)
        {
            try
            {
                using var dbContext = new MemberInfoDbContext();
                searchTerm = searchTerm.ToLower().Trim();
                return dbContext.MemberInfo.Where(m =>
                    m.MemberName.ToLower().Contains(searchTerm) ||
                    m.EmailID.ToLower().Contains(searchTerm) ||
                    string.Equals(searchTerm, Convert.ToString(m.RegistrationID))
                ).ToList();
               
            }
            catch
            {
                return Enumerable.Empty<MemberInfo>();
            }
        }
    }
}