Shader "Custom/DisableZWriteRevealX"
{
    Properties
    {
        _RevealX ("Reveal X", Float) = 0.0  // Contrôle la progression du dévoilement
        _Color ("Color", Color) = (1,1,1,1) // Couleur de l'objet (optionnel)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            ZWrite Off
            
        }
    }
}
