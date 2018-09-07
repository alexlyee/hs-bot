using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HSBot.Helpers;

namespace HSBot.Modules.References
{
    public class GroupType
    {
        public ulong id;
        public string name;
    }
    public class GroupClass : GroupType
    {
        public Hour[] hours;
        public Teacher teacher;
        public ulong[] roles;

        public Subject? subject;
        public ulong? channel;

        public GroupClass(Hour[] hours, Teacher teacher, ulong[] roles, Subject? subject, ulong? channel)
        {
            if (subject.Equals(null) && channel.Equals(null))
            {
                Utilities.Log(MethodBase.GetCurrentMethod(), "Failed to create class without subject or channel.");
                return;
            }
            if (!subject.Equals(null) && !channel.Equals(null))
            {
                Utilities.Log(MethodBase.GetCurrentMethod(), "Failed to create class with both subject and channel.");
                return;
            }

        }
    }
    class GroupOverseen : GroupType
    {

    }
    class GroupInherent : GroupType
    {

    }
    class GroupIdiomatic : GroupType
    {

    }
    class GroupFunctional : GroupType
    {

    }
}
