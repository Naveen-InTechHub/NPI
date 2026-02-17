using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPI.Shared.Enums
{
    public struct Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string NPIManager = "NPIManager";
        public const string Planner = "Planner";
        public const string QualityOfficer = "QualityOfficer";
        public const string PackagingExec = "PackagingExec";
    }
    public struct NotesParent
    {
        public const string Notes = "Notes";
        public const string Setup = "Setup";
        public const string Pilot = "Pilot";
    }
}
