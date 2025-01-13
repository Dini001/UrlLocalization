namespace Example1;

public class Cultures
{
    // Could also be only the language ie: ["fr", "en"]
    // Use the casing desired for the url

    public const string DEFAUT_CULTURE = "fr-CA";
    public const string OTHER_CULTURE_1 = "en-CA";
    //public const string OTHER_CULTURE_2 = "...";
    // ...

    public static readonly string[] SupportedCultures = [DEFAUT_CULTURE, OTHER_CULTURE_1 /* , OTHER_CULTURE_2, ...  */];

}
