namespace MyHealthNotebook.Configurations.Messages
{
    public static class ErrorMessages
    {
        public static class Generic
        {
            public static string SomethingWentWrong = "Something went wrong, please try again later";
            public static string TypeUnableToProcess = "Unable to process request";
            public static string TypeBadRequest = "Bad Request";
            public static string InvalidPayload = "Invalid Payload";
            public static string InvalidRequest = "Invalid Request";
            public static string ObjectNotFound = "Object Not Found";
        }

        public static class Profile
        {
            public static string ProfileNotFound = "Profile not found";
        }

        public static class Users
        {
            public static string UserNotFound = "User not found";
        }
    }
}