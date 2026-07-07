using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace IspsDashboard.Infrastructure;

/// <summary>
/// Supprime le [Required] implicite ajouté par ASP.NET sur les types référence
/// non-nullable (dû à &lt;Nullable&gt;enable&lt;/Nullable&gt;). Seuls les [Required]
/// explicites restent appliqués — évite les faux « champ obligatoire » sur les
/// propriétés optionnelles.
/// </summary>
public sealed class DisableImplicitRequiredProvider : IValidationMetadataProvider
{
    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        if (context.Key.MetadataKind != ModelMetadataKind.Property)
            return;

        var explicitRequired = context.PropertyAttributes?
            .OfType<RequiredAttribute>().FirstOrDefault();

        if (explicitRequired is null)
        {
            // Aucun [Required] explicite : retirer le Required implicite (NRT),
            // côté serveur ET côté client (data-val-required).
            var implicitRequired = context.ValidationMetadata.ValidatorMetadata
                .OfType<RequiredAttribute>().ToList();
            foreach (var attr in implicitRequired)
                context.ValidationMetadata.ValidatorMetadata.Remove(attr);
            context.ValidationMetadata.IsRequired = false;
        }
        else if (string.IsNullOrEmpty(explicitRequired.ErrorMessage)
                 && string.IsNullOrEmpty(explicitRequired.ErrorMessageResourceName))
        {
            // [Required] explicite sans message : message français clair.
            explicitRequired.ErrorMessage = "Ce champ est obligatoire.";
        }
    }
}
