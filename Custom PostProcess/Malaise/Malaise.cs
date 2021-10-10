using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(MalaiseRenderer), PostProcessEvent.AfterStack, "Hidden/Cutsom/Malaise")]
public sealed class Malaise : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Effet de zone de mort")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
    [Range(0f, 1f), Tooltip("Distortion Time")]
    public FloatParameter dist = new FloatParameter { value = 0.1f };
    [Tooltip("Texture Overlay")]
    public TextureParameter texture = new TextureParameter { value = null };
}

public sealed class MalaiseRenderer : PostProcessEffectRenderer<Malaise>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Malaise"));

        var imageTexture = settings.texture.value == null    // Si il n'y a pas texture 
            ? RuntimeUtilities.transparentTexture            // Ajout d'une texture transparente
            : settings.texture.value;                        // Mais si la premiere ligne = false alors on prend la texture depuis la class Malaise

        sheet.properties.SetTexture("_TextureNoise", imageTexture);

        sheet.properties.SetFloat("_Distortion", settings.dist);
        sheet.properties.SetFloat("_Blend", settings.blend);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}