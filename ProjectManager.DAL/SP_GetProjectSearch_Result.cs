//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectManager.DAL
{
    using System;
    
    public partial class SP_GetProjectSearch_Result
    {
        public int Project_ID { get; set; }
        public string ProjectName { get; set; }
        public string Project_Start_Date { get; set; }
        public string Project_End_Date { get; set; }
        public string Project_Priority { get; set; }
        public Nullable<int> User_ID { get; set; }
        public Nullable<int> UserProjectID { get; set; }
        public Nullable<int> UserEmployeeID { get; set; }
        public string UserFirstName { get; set; }
        public string UserLAstName { get; set; }
        public string UserFullName { get; set; }
        public Nullable<int> No_OfTask { get; set; }
        public Nullable<int> No_OfTaskCompleted { get; set; }
    }
}
