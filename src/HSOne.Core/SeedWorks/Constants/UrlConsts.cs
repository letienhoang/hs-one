namespace HSOne.Core.SeedWorks.Constants
{
    public static class UrlConsts
    {
        // Site
        public static string Home = "/";
        public static string About = "/about";
        public static string Contact = "/contact";

        // Content
        public static string Posts = "/posts";
        public static string PostsByCategorySlug = "/posts/{0}";
        public static string PostDetail = "/post/{0}";
        public static string PostsByTagSlug = "/tag/{0}";
        public static string Author = "/author/{0}";
        public static string Series = "/series";
        public static string SeriesDetail = "/series/{0}";

        // User
        public static string Login = "/login";
        public static string Register = "/register";
        public static string ForgotPassword = "/forgot-password";
        public static string ResetPassword = "/reset-password";

        // Profile
        public static string Profile = "/profile";
        public static string ChangePassword = "profile/change-password";
        public static string ChangeProfile = "/profile/edit";
        public static string PostListByUser = "/profile/posts/list";
        public static string CreatePost = "/profile/posts/create-post";
    }
}
