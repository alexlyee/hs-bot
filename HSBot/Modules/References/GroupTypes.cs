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
        public ulong[] roles; // Not to be edited directly by users.

        public Subject? subject; // Can be none.
        public ulong[] channels; // Can be none.
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
