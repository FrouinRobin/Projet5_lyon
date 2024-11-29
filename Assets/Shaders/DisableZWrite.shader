Shader "Custom/DisableZWriteRevealX"
{
    Properties
    {
        _RevealX ("Reveal X", Float) = 0.0  // Contr�le la progression du d�voilement
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
