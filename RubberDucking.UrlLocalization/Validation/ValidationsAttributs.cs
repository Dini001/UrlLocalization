using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using RubberDucking.UrlLocalization.Attributes;
using System.Diagnostics;
using System.Reflection;

namespace RubberDucking.UrlLocalization.Validation;

/// <summary>
/// TO DO : Refactoring to be use in units test instead
/// TO DO : Add support for inheritance of controller
/// </summary>
public static class AttributesValidation
{

    [Conditional("DEBUG")]
    public static void ValidateAttributes<T>(string[] requiredCulture)
    {
        var controllers = typeof(T).Assembly
                            .GetTypes()
                            .Where(type => typeof(Controller).IsAssignableFrom(type))
                            .Select(p => p);

        foreach (var controller in controllers)
            ValidateControllers(controller, requiredCulture);
    }

    private static void ValidateControllers(Type controller, string[] requiredCulture)
    {
        var LocalizedControllerNames = controller.GetCustomAttributes<LocalizedControllerNameAttribute>();
        if (LocalizedControllerNames != null && LocalizedControllerNames.Any())
            ValidateControllerAttributes(controller.Name, LocalizedControllerNames, requiredCulture);
        else
            _ = controller.GetCustomAttribute<WithoutLocalizedNameAttribute>()
                ?? throw new Exception($"No localization attribute was found on the controller \"{controller.Name}\". Please add an [WithoutLocalizedNameAttribute] if this is a url that should not be localized.");

        ValidateActions(controller, requiredCulture);
    }

    private static void ValidateControllerAttributes(string controllerName, IEnumerable<LocalizedControllerNameAttribute> localizedControllerNames, string[] requiredCulture)
    {
        var cultures = localizedControllerNames.Select(nom => nom.Culture).ToArray();
        var missings = requiredCulture.Except(cultures);
        if (missings.Any())
        {
            var missingCultures = missings.ToArray();
            if (missingCultures.Length == 1)
                throw new Exception($"The culture \"{missingCultures[0]}\" is missing for the controller \"{controllerName}\"");
            else
                throw new Exception($"The culture \"{string.Join("\", \"", missingCultures)}\" are missing for the controller \"{controllerName}\"");
        }

        var surplus = cultures.Except(requiredCulture);
        if (surplus.Any())
        {
            var cultureSurplus = surplus.ToArray();
            if (cultureSurplus.Length == 1)
                throw new Exception($"The culture \"{cultureSurplus[0]}\" is in surplus on the controller \"{controllerName}\"");
            else
                throw new Exception($"The culture \"{string.Join("\", \"", cultureSurplus)}\" are in surplus on the controller \"{controllerName}\"");
        }
    }

    private static void ValidateActions(Type controller, string[] requiredCulture)
    {
        var methods = controller
                        .GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                        .Where(m => !m.IsSpecialName);//Retirer les propriétés
        var translationsMethods = new List<TranslationMethod>();
        foreach (var method in methods)
            ValidateAction(controller, method, translationsMethods, requiredCulture);
        if (translationsMethods.Distinct().Count() != translationsMethods.Count)
            throw new Exception($"Duplicate translation in the controller \"{controller.Name}\"");
    }

    private static readonly Type[] typesMethod = [typeof(HttpGetAttribute), typeof(HttpPostAttribute), typeof(HttpPutAttribute), typeof(HttpDeleteAttribute), typeof(HttpOptionsAttribute), typeof(HttpPatchAttribute)];
    private static void ValidateAction(Type controller, MethodInfo method, List<TranslationMethod> translationMethods, string[] requiredCulture)
    {
        var localizedActionNames = method.GetCustomAttributes<LocalizedActionNameAttribute>();
        if (localizedActionNames != null && localizedActionNames.Any())
        {
            ValidateActionAttributes(controller.Name, method.Name, localizedActionNames, requiredCulture);
            var hasAttribute = false;
            foreach (var type in typesMethod)
            {
                if (method.GetCustomAttribute(type) is not HttpMethodAttribute attribut)
                    continue;

                hasAttribute = true;
                translationMethods.AddRange(localizedActionNames.Select(a => new TranslationMethod()
                {
                    Culture = a.Culture,
                    Method = attribut.HttpMethods.First(),
                    Translation = a.ExcludeActionName ? "Index" : (a.Name ?? string.Empty)
                }));
                break;//having more than 1 of these attribut would not work
            }
            if (!hasAttribute)
            {
                var attribute = method.GetCustomAttribute<AcceptVerbsAttribute>();
                if (attribute != null)
                {
                    hasAttribute = true;
                    translationMethods.AddRange(from a in localizedActionNames
                                                from attr in attribute.HttpMethods
                                                select new TranslationMethod()
                                                {
                                                    Culture = a.Culture,
                                                    Method = attr,
                                                    Translation = a.ExcludeActionName ? "Index" : (a.Name ?? string.Empty)
                                                });
                }
            }
            if (!hasAttribute)
                translationMethods.AddRange(localizedActionNames.Select(a => new TranslationMethod()
                {
                    Culture = a.Culture,
                    Method = null,
                    Translation = a.ExcludeActionName ? "Index" : (a.Name ?? string.Empty)
                }));
        }
        else
            _ = method.GetCustomAttribute<WithoutLocalizedNameAttribute>()
                ?? throw new Exception($"Aucun attribut de localisation n'a été trouvé pour l'action \"{method.Name}\" du controller \"{controller.Name}\". Ajouter un [SansNomLocaliseAttribute] si ce n'est pas une url qui doit être localisé.");
    }

    private static void ValidateActionAttributes(string nomController, string nomAction, IEnumerable<LocalizedActionNameAttribute> nomsActionLocalises, string[] requiredCulture)
    {
        var cultures = nomsActionLocalises.Select(nom => nom.Culture).ToArray();
        var missing = requiredCulture.Except(cultures);
        if (missing.Any())
        {
            var missginCulture = missing.ToArray();
            if (missginCulture.Length == 1)
                throw new Exception($"The culture \"{missginCulture[0]}\" is missing for the action \"{nomAction}\" on the controller \"{nomController}\"");
            else
                throw new Exception($"The culture \"{string.Join("\", \"", missginCulture)}\" are missing for the action \"{nomAction}\" on the controller \"{nomController}\"");
        }

        var surplus = cultures.Except(requiredCulture);
        if (surplus.Any())
        {
            var cultureSurplus = surplus.ToArray();
            if (cultureSurplus.Length == 1)
                throw new Exception($"The culture \"{cultureSurplus[0]}\" is in surplus for the action \"{nomAction}\" on the controller \"{nomController}\"");
            else
                throw new Exception($"The culture \"{string.Join("\", \"", cultureSurplus)}\" are in surplus for the action \"{nomAction}\" on the controller \"{nomController}\"");
        }
    }

    private sealed record TranslationMethod
    {
        public required string Culture { get; init; }
        public required string? Method { get; init; }
        public required string Translation { get; init; }
    }
}
