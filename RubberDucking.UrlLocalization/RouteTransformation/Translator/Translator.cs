using Microsoft.AspNetCore.Mvc;
using RubberDucking.UrlLocalization.Attributes;
using System.Reflection;

namespace RubberDucking.UrlLocalization.RouteTransformation.Translator;

public static class Translator
{
    private static readonly List<TranslationControllerItem> translations = GetTranslationsUrl();

    public static void GetTranslation(ref RoutesSegments routesSegments)
    {
        if (string.IsNullOrEmpty(routesSegments.Culture) || string.IsNullOrEmpty(routesSegments.Controller))
            return;

        TranslationControllerItem? translationController = null;
        foreach (var translation in translations) //translations.FirstOrDefault(/**/)
        {
            if (translation.Culture.Equals(routesSegments.Culture, StringComparison.InvariantCultureIgnoreCase) &&
                translation.OriginalName.Equals(routesSegments.Controller, StringComparison.InvariantCultureIgnoreCase))
            {
                translationController = translation;
                break;
            }
        }

        if (translationController == null)
            return;

        routesSegments.Controller = translationController.Name;
        if (string.IsNullOrEmpty(routesSegments.Action))
            return;

        TranslationActionItem? translationAction = null;
        foreach (var translation in translationController.TranslationActions) //translationController.TranslationActions.FirstOrDefault(/**/)
        {
            if (translation.OriginalName.Equals(routesSegments.Action, StringComparison.InvariantCultureIgnoreCase))
            {
                translationAction = translation;
                break;
            }
        }

        if (translationAction == null)
            return;

        routesSegments.Action = translationAction.Name;
    }

    public static void Resolve(ref RoutesSegments routesSegments)
    {
        if (string.IsNullOrEmpty(routesSegments.Controller))
            return;

        TranslationControllerItem? translationController = null;
        foreach (var translation in translations) // translations.FirstOrDefault(/**/)
        {
            if (translation.Culture.Equals(routesSegments.Culture, StringComparison.InvariantCultureIgnoreCase) &&
                translation.Name.Equals(routesSegments.Controller, StringComparison.InvariantCultureIgnoreCase))
            {
                translationController = translation;
                break;
            }
        }

        if (translationController == null)
            return;

        routesSegments.Controller = translationController.OriginalName;
        if (string.IsNullOrEmpty(routesSegments.Action))
            return;

        TranslationActionItem? translationAction = null;
        foreach (var translation in translationController.TranslationActions) //traductionController.TraductionsActions.FirstOrDefault(/**/)
        {
            if (translation.Name.Equals(routesSegments.Action, StringComparison.InvariantCultureIgnoreCase))
            {
                translationAction = translation;
                break;
            }
        }

        if (translationAction == null)
            return;

        routesSegments.Action = translationAction.OriginalName;
    }

    private static List<TranslationControllerItem> GetTranslationsUrl()
    {
        var translations = new List<TranslationControllerItem>();
        var assembly = Assembly.GetEntryAssembly() ?? throw new NullReferenceException("Aucune assembly n'a été trouvé");
        var controllers = assembly
                            .GetTypes()
                            .Where(type => typeof(Controller).IsAssignableFrom(type))
                            .Select(p => p);

        foreach (var controller in controllers)
        {
            var controllerName = controller.Name[..^10];
            var localizedControllerNames = controller.GetCustomAttributes<LocalizedControllerNameAttribute>();
            if (localizedControllerNames != null)
                foreach (var localizedControllerName in localizedControllerNames)
                {
                    translations.Add(new TranslationControllerItem()
                    {
                        Culture = localizedControllerName.Culture,
                        Name = localizedControllerName.Name,
                        OriginalName = controllerName
                    });
                }

            var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var localizedActionNames = method.GetCustomAttributes<LocalizedActionNameAttribute>();
                if (localizedActionNames == null)
                    continue;

                foreach (var localizedActionName in localizedActionNames)
                {
                    var translationControllerItem = translations.FirstOrDefault(p => p.Culture.Equals(localizedActionName.Culture, StringComparison.InvariantCultureIgnoreCase) &&
                                                                                    p.OriginalName.Equals(controllerName, StringComparison.InvariantCultureIgnoreCase));
                    if (translationControllerItem == null)
                    {
                        translationControllerItem = new TranslationControllerItem()
                        {
                            Culture = localizedActionName.Culture,
                            Name = controllerName,
                            OriginalName = controllerName
                        };
                        translations.Add(translationControllerItem);
                    }
                    else
                    {
                        var localizedname = localizedActionName.ExcludeActionName ? "Index" : localizedActionName.Name ?? string.Empty;
                        if (translationControllerItem.TranslationActions.Any(ai => ai.OriginalName == method.Name && ai.Name == localizedname))
                            continue;

                        translationControllerItem.TranslationActions.Add(new TranslationActionItem()
                        {
                            Name = localizedname,
                            ExcludeActionName = localizedActionName.ExcludeActionName,
                            OriginalName = method.Name
                        });
                    }
                }
            }
        }
        return translations;
    }
}
