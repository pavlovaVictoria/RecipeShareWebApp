namespace RecipeShare.Common
{
    public class EntityValidationMessages
    {
        //Recipe Messages
        public const string ErrorMessageRecipeTitle = "The length of the Title of the Recipe must be between 5 and 20";
        public const string ErrorMessageRecipeDescription = "The length of the Description must be between 10 and 30";
        public const string ErrorMessageRecipePreparation = "The length of the Preparation must be between 100 and 500";
        public const string ErrorMessageDate = "The pattern of the DateTime is not correct";

        //Product Messages
        public const string ErrorMessageProductName = "The length of the Product Name must be between 3 and 20";

        //Recipe Category Messages
        public const string ErrorMessageCategoryName = "The length of the Category Name must be between 3 and 20";

        //Allergen Messages
        public const string ErrorMessageAllergenName = "The length of the Allergen Name must be between 3 and 20";

        //Comment Messages
        public const string ErrorMessageCommentText = "The max length is 250";

        //Application User Messages
        public const string ErrorMessageUserBio = "The max length is 20";
        public const string ErrorMessageUserName = "The length of the UserName must be between 3 and 20";
        public const string ErrorMessagePassword = "Password must be 8-20 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character.";
        public const string ErrorMessageConfirmedPassword = "Password does noe match";
    }
}
