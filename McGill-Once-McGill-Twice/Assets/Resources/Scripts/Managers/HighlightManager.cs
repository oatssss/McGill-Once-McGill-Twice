using UnityEngine;
using System.Collections.Generic;

public class HighlightManager : UnitySingletonPersistent<HighlightManager> {

	public HighlightsPostEffect.HighlightType m_selectionType = HighlightsPostEffect.HighlightType.Glow;
	public HighlightsPostEffect.SortingType m_sortingType = HighlightsPostEffect.SortingType.DepthFilter;
	public HighlightsPostEffect.FillType m_fillType = HighlightsPostEffect.FillType.Outline;
	public HighlightsPostEffect.RTResolution m_resolution = HighlightsPostEffect.RTResolution.Full;

	public string m_occludeesTag = "Occludee";
	public string m_occludersTag = "Occluder";
	public Color m_highlightColor = new Color(1f, 1f, 0f, 1f);

	public Shader m_highlightShader;


	public List<Renderer> highlightObjects = new List<Renderer>();
	public List<Renderer> m_occluders = new List<Renderer>();

    public void Highlight(GameObject go, bool highlight)
    {
        Renderer renderer = go.GetComponent<Renderer>();
        if (highlight && !highlightObjects.Contains(renderer))
        {
            highlightObjects.Add(renderer);
        }
        else
        {
            highlightObjects.Remove(renderer);
        }
    }
}
