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
    }
}
