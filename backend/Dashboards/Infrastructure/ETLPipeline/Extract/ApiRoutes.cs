namespace Infrastructure.ETLPipeline.Extract
{
    internal static class ApiRoutes
    {
        public const string ApiAuthUrl = "https://dev-lk-api.univuz.ru/auth/clienttoken";
        public const string CitizenshipUrl = "https://dev-lk-api.univuz.ru/contingent/d/citizenship";
        public const string FacultyUrl = "https://dev-lk-api.univuz.ru/contingent/department/GetAllFaculty/2";
        public const string StudentAcademicStateUrl = "https://dev-lk-api.univuz.ru/contingent/d/studentacademicstate";
        public const string StudyFormUrl = "https://dev-lk-api.univuz.ru/contingent/d/common/getalldictstudyform";
        public const string ContingentUrl = "https://dev-lk-api.univuz.ru/contingent/contingent/GetContingentPage";
        public const string EducationProgramUrl = "https://dev-lk-api.univuz.ru/contingent/education/GetAllEducationProgram";
        public const string EducationStandardUrl = "https://dev-lk-api.univuz.ru/contingent/education/GetAllEducationStandard";
        public const string BenefitUrl = "https://dev-lk-api.univuz.ru/contingent/d/benefit";
        public const string OrganizationUrl = "https://dev-lk-api.univuz.ru/contingent/d/organization";
        public const string AchivmentsUrl = "https://dev-lk-api.univuz.ru//scholarship/portfolio/achievements";
        public const string AchivmentCategoryUrl = "https://dev-lk-api.univuz.ru//scholarship/dicts/achievement-categories";
        public const string OrderUrl = "https://dev-lk-api.univuz.ru/contingent/manualOrders";
        public const string OrderCategoryUrl = "https://dev-lk-api.univuz.ru/contingent/d/orderscategory/GetAllDictOrdersCategory/true";
        public const string BranchUrl = "https://dev-lk-api.univuz.ru/contingent/d/filial";
        public const string TrainingLevelUrl = "https://dev-lk-api.univuz.ru/contingent/d/traininglevels/GetList";
        public const string DisciplinesUrl = "https://dev-lk-api.univuz.ru/session/d/Disciplines";
        public const string MarksUrl = "https://dev-lk-api.univuz.ru/session/d/Marks";
        public const string GroupUrl = "https://dev-lk-api.univuz.ru//scholarship/edu-groups";
        public const string SheetDisciplineUrl = "https://dev-lk-api.univuz.ru/session/sheetDiscipline/GetStudents";
        public const string PlanUrl = "https://dev-lk-api.univuz.ru//session/sheetprint/GetSheetHeader";
    }
}
