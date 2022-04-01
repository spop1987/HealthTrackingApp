namespace MyHealthNotebook.Configurations.Messages
{
    public static class ErrorMessages
    {
        public static class Generic
        {
            public static string SomethingWentWrong = "Something went wrong, please try again later";
            public static string TypeUnableToProcess = "Unable to process request";
            public static string TypeBadRequest = "Bad Request";
            public static string InvalidPayload = "Invalid payload";
        }

        public static class Profile
        {
            public static string UserNotFound = "User not found";
            public static string ProfileNotFound = "Profile not found";
        }
    }
}