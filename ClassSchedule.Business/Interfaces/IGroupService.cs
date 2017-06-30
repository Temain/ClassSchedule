using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassSchedule.Domain.Models;

namespace ClassSchedule.Business.Interfaces
{
    public interface IGroupService
    {
        IQueryable<Group> GetEditableGroups(string userId);

        List<int> GetEditableGroupsIdentifiers(string userId);
    }
}
