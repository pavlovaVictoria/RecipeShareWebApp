namespace RecipeShare.Common
{
    public class ApplicationConstants
    {
        //Recipe Constants
        public const int RecipeTitleMinLen = 5;
        public const int RecipeTitleMaxLen = 20;

        public const int RecipeDescriptionMinLen = 10;
        public const int RecipeDescriptionMaxLen = 30;

        public const int RecipePreparationMinLen = 100;
        public const int RecipePreparationMaxLen = 500;

        public const string RecipeReleaseDatePattern = "yyyy/MM/dd HH:mm";
        public const string RegexDateTimePattern = @"^\d{4}/(0[1-9]|1[0-2])/(0[1-9]|[12][0-9]|3[01])\s([01][0-9]|2[0-3]):[0-5][0-9]$";

        //Product Constants
        public const int ProductNameMinLen = 3;
        public const int ProductNameMaxLen = 20;

        //Recipe Category Constants
        public const int CategoryNameMinLen = 3;
        public const int CategoryNameMaxLen = 20;

        //Allergen Constants
        public const int AllergenNameMinLen = 3;
        public const int AllergenNameMaxLen = 20;

        //Comment Constants
        public const int CommentTextMaxLen = 250;

        //Application User Constants
        public const int UserBioMax = 20;
        public const int UserNameMinLen = 3;
        public const int UserNameMaxLen = 20;
        public const string RegexForPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,12}$";

        //Decimal
        public const int NumberOfDigits = 8;
        public const int NumbersAfter = 2;
    }
}
