using CityCentralLibrary.DAL.Models;

namespace CityCentralLibrary.DAL.Repositories
{
    public interface IRepository
    {
        bool RegisterForMembership(MemberInfo member);
        bool CancelMembership(int registrationId);
        bool UpdateEmail(int registrationId, string email);
        IEnumerable<MemberInfo> GetAllMembers();
        IEnumerable<MemberInfo> SearchMember(string searchTerm);
    }
}
